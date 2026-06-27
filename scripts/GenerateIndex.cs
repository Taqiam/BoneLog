#:project ../src/BoneLog/BoneLog.csproj
#:property JsonSerializerIsReflectionEnabledByDefault=true

using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BoneLog.Models;
using BoneLog.Tools;

#region Entry

var options = CliOptions.Parse(args);
if (options is null)
{
    Console.Error.WriteLine("Usage: dotnet run GenerateIndex.cs -- <rootPath> <index.json> [--full]");
    Console.Error.WriteLine("  Default: incremental update (hash-checked). Use --full to rebuild all posts.");
    Environment.Exit(1);
}

var rootPath = Path.GetFullPath(options.RootPath);
var indexOut = Path.GetFullPath(options.IndexPath);
var manifestOut = Path.ChangeExtension(indexOut, ".manifest.json");
var logOut = Path.ChangeExtension(indexOut, ".generation.log.json");

if (!Directory.Exists(rootPath))
{
    Console.Error.WriteLine($"Root path not found: {rootPath}");
    Environment.Exit(1);
}

var stopwatch = Stopwatch.StartNew();
var report = new GenerationReport
{
    StartedAt = DateTimeOffset.UtcNow,
    Mode = options.Full ? "full" : "incremental",
    RootPath = rootPath,
    IndexPath = indexOut
};

try
{
    var result = options.Full
        ? IndexBuilder.BuildFull(rootPath, report)
        : await IndexBuilder.BuildIncrementalAsync(rootPath, indexOut, manifestOut, report);

    report.DurationMs = stopwatch.ElapsedMilliseconds;
    report.PostCount = result.Posts.Count;
    report.LanguageCount = result.Posts.SelectMany(static p => p.Languages).Distinct(StringComparer.OrdinalIgnoreCase).Count();
    report.FilesProcessed = result.FilesProcessed;
    report.FilesSkipped = result.FilesSkipped;
    report.UpdatedCount = result.UpdatedCount;
    report.KeptCount = result.KeptCount;
    report.RemovedCount = result.RemovedCount;
    report.FinishedAt = DateTimeOffset.UtcNow;

    await JsonWriter.WriteAsync(indexOut, result.Posts);
    await JsonWriter.WriteAsync(manifestOut, result.Manifest);
    await JsonWriter.WriteAsync(logOut, report);

    WriteConsoleSummary(report, result);

    if (report.Errors.Count > 0)
        Environment.Exit(1);
}
catch (Exception ex)
{
    report.DurationMs = stopwatch.ElapsedMilliseconds;
    report.FinishedAt = DateTimeOffset.UtcNow;
    report.Errors.Add(ex.Message);
    await JsonWriter.WriteAsync(logOut, report);
    Console.Error.WriteLine($"Fatal: {ex.Message}");
    Environment.Exit(1);
}

static void WriteConsoleSummary(GenerationReport report, BuildResult result)
{
    foreach (var warning in report.Warnings)
        Console.WriteLine($"WARNING: {warning}");

    foreach (var error in report.Errors)
        Console.Error.WriteLine($"ERROR: {error}");

    Console.WriteLine(
        report.Mode == "full"
            ? $"Full build: wrote {result.Posts.Count} posts -> {report.IndexPath}"
            : $"Updated {result.UpdatedCount}, kept {result.KeptCount}, removed {result.RemovedCount} -> {report.IndexPath}");

    Console.WriteLine($"Generation log: {Path.ChangeExtension(report.IndexPath, ".generation.log.json")}");
    Console.WriteLine($"Duration: {report.DurationMs} ms");
}

#endregion

#region Models

sealed record CliOptions(string RootPath, string IndexPath, bool Full)
{
    public static CliOptions? Parse(string[] args)
    {
        var full = false;
        var positional = new List<string>(args.Length);

        foreach (var arg in args)
        {
            if (arg is "--full" or "-f")
                full = true;
            else if (arg is "--help" or "-h")
                return null;
            else
                positional.Add(arg);
        }

        if (positional.Count != 2)
            return null;

        return new CliOptions(positional[0], positional[1], full);
    }
}

sealed class GenerationReport
{
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }
    public long DurationMs { get; set; }
    public string Mode { get; set; } = "";
    public string RootPath { get; set; } = "";
    public string IndexPath { get; set; } = "";
    public int PostCount { get; set; }
    public int LanguageCount { get; set; }
    public int FilesProcessed { get; set; }
    public int FilesSkipped { get; set; }
    public int UpdatedCount { get; set; }
    public int KeptCount { get; set; }
    public int RemovedCount { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public List<string> Info { get; set; } = [];
}

sealed class IndexManifest
{
    public Dictionary<string, string> Hashes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

sealed class BuildResult
{
    public required List<PostIndex> Posts { get; init; }
    public required IndexManifest Manifest { get; init; }
    public int UpdatedCount { get; init; }
    public int KeptCount { get; init; }
    public int RemovedCount { get; init; }
    public int FilesProcessed { get; init; }
    public int FilesSkipped { get; init; }
}

sealed class ParsedVariant
{
    public required string FilePath { get; init; }
    public required string Directory { get; init; }
    public required string BaseName { get; init; }
    public required string Language { get; init; }
    public PostFrontMatter? Header { get; init; }
    public string? Id { get; set; }
    public string? Slug { get; set; }
}

#endregion

#region IndexBuilder

static class IndexBuilder
{
    public static BuildResult BuildFull(string rootPath, GenerationReport report)
    {
        var root = new DirectoryInfo(rootPath);
        var variants = ParseAllVariants(root, report);
        var posts = BuildGroupedPosts(variants, report);
        var manifest = new IndexManifest();

        foreach (var variant in variants)
            manifest.Hashes[variant.FilePath] = HashHelper.ComputeFileHash(Path.Combine(root.FullName, variant.FilePath + ".md"));

        SortPosts(posts);
        report.Info.Add($"Indexed {posts.Count} posts from {variants.Count} files.");

        return new BuildResult
        {
            Posts = posts,
            Manifest = manifest,
            UpdatedCount = posts.Count,
            KeptCount = 0,
            RemovedCount = 0,
            FilesProcessed = variants.Count,
            FilesSkipped = report.FilesSkipped
        };
    }

    public static async Task<BuildResult> BuildIncrementalAsync(string rootPath, string indexPath, string manifestPath, GenerationReport report)
    {
        if (!File.Exists(indexPath) || !File.Exists(manifestPath))
            return BuildFull(rootPath, report);

        var existingManifest = await JsonReader.ReadAsync<IndexManifest>(manifestPath) ?? new IndexManifest();
        var root = new DirectoryInfo(rootPath);
        var variants = ParseAllVariants(root, report);
        var posts = BuildGroupedPosts(variants, report);
        var manifest = new IndexManifest();

        var kept = 0;
        foreach (var variant in variants)
        {
            var fullPath = Path.Combine(root.FullName, variant.FilePath + ".md");
            var hash = HashHelper.ComputeFileHash(fullPath);
            manifest.Hashes[variant.FilePath] = hash;

            if (existingManifest.Hashes.TryGetValue(variant.FilePath, out var previousHash)
                && string.Equals(previousHash, hash, StringComparison.OrdinalIgnoreCase))
            {
                kept++;
            }
        }

        SortPosts(posts);
        report.Info.Add($"Indexed {posts.Count} posts from {variants.Count} files.");

        return new BuildResult
        {
            Posts = posts,
            Manifest = manifest,
            UpdatedCount = variants.Count - kept,
            KeptCount = kept,
            RemovedCount = Math.Max(0, existingManifest.Hashes.Count - manifest.Hashes.Count),
            FilesProcessed = variants.Count,
            FilesSkipped = report.FilesSkipped
        };
    }

    static List<ParsedVariant> ParseAllVariants(DirectoryInfo root, GenerationReport report)
    {
        var variants = new List<ParsedVariant>();

        foreach (var file in root.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            if (!TryParseVariant(file, root, out var variant, report))
            {
                report.FilesSkipped++;
                continue;
            }

            variants.Add(variant);
        }

        ResolveVariantIdentity(variants, report);
        return variants;
    }

    static List<PostIndex> BuildGroupedPosts(List<ParsedVariant> variants, GenerationReport report)
    {
        var groups = variants
            .Where(static v => !string.IsNullOrWhiteSpace(v.Id))
            .GroupBy(static v => v.Id!, StringComparer.OrdinalIgnoreCase)
            .ToList();

        ValidateGroups(groups, report);

        if (report.Errors.Count > 0)
            return [];

        var posts = groups.Select(g => MergeGroup(g.ToList(), report)).Where(static p => p is not null).Cast<PostIndex>().ToList();
        WarnDuplicateSlugs(posts, report);
        return posts;
    }

    static void ValidateGroups(List<IGrouping<string, ParsedVariant>> groups, GenerationReport report)
    {
        foreach (var group in groups)
        {
            var slugs = group.Select(static v => v.Slug).Where(static s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (slugs.Count > 1)
            {
                report.Errors.Add(
                    $"Duplicate id '{group.Key}' used with different slugs: {string.Join(", ", slugs)}.");
            }

            var duplicateLanguages = group
                .GroupBy(static v => v.Language, StringComparer.OrdinalIgnoreCase)
                .Where(static g => g.Count() > 1)
                .Select(static g => g.Key)
                .ToList();

            if (duplicateLanguages.Count > 0)
            {
                report.Errors.Add(
                    $"Post id '{group.Key}' has multiple files for language(s): {string.Join(", ", duplicateLanguages)}.");
            }
        }

        var idsByBaseKey = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var variant in variantsWithIdentity(groups))
        {
            var key = $"{variant.Directory}/{variant.BaseName}";
            if (!idsByBaseKey.TryGetValue(key, out var ids))
            {
                ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                idsByBaseKey[key] = ids;
            }

            ids.Add(variant.Id!);
            if (ids.Count > 1)
            {
                report.Errors.Add(
                    $"Same file name '{variant.BaseName}' in '{variant.Directory}' is used by different ids: {string.Join(", ", ids)}.");
            }
        }
    }

    static IEnumerable<ParsedVariant> variantsWithIdentity(List<IGrouping<string, ParsedVariant>> groups) =>
        groups.SelectMany(static g => g);

    static void WarnDuplicateSlugs(IReadOnlyList<PostIndex> posts, GenerationReport report)
    {
        var slugGroups = posts
            .GroupBy(static p => p.Slug, StringComparer.OrdinalIgnoreCase)
            .Where(static g => g.Count() > 1);

        foreach (var group in slugGroups)
        {
            var ids = string.Join(", ", group.Select(static p => p.Id));
            report.Warnings.Add($"Slug '{group.Key}' is shared by multiple posts: {ids}.");
        }
    }

    static PostIndex? MergeGroup(List<ParsedVariant> variants, GenerationReport report)
    {
        var metadataSource = variants.FirstOrDefault(static v => v.Language == PathExtensions.DefaultLanguage)
            ?? variants.FirstOrDefault(static v => HasRequiredMetadata(v))
            ?? variants[0];

        if (!HasRequiredMetadata(metadataSource))
        {
            report.Errors.Add(
                $"Post '{metadataSource.FilePath}' is missing required metadata. English file or a complete front matter (id, slug, categoryPath) is required.");
            return null;
        }

        var header = metadataSource.Header ?? new PostFrontMatter
        {
            Id = metadataSource.Id,
            Slug = metadataSource.Slug
        };

        var localizedTitles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var filePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var variant in variants.OrderBy(static v => v.Language, StringComparer.OrdinalIgnoreCase))
        {
            filePaths[variant.Language] = variant.FilePath;

            if (!variant.Language.Equals(PathExtensions.DefaultLanguage, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(variant.Header?.Title))
            {
                localizedTitles[variant.Language] = variant.Header!.Title!;
            }
        }

        return new PostIndex
        {
            Id = metadataSource.Id!,
            Slug = metadataSource.Slug!,
            Title = string.IsNullOrWhiteSpace(header.Title)
                ? metadataSource.BaseName.SlugToTitle()
                : header.Title!,
            ShortDescription = header.ShortDescription,
            Tags = header.Tags,
            Thumbnail = header.Thumbnail,
            Category = header.CategoryPath,
            Date = PostFrontMatter.ParseDate(header.Date),
            Languages = filePaths.Keys.OrderBy(static l => l, StringComparer.OrdinalIgnoreCase).ToArray(),
            LocalizedTitles = localizedTitles.Count > 0 ? localizedTitles : null,
            FilePaths = filePaths
        };
    }

    static bool HasRequiredMetadata(ParsedVariant variant) =>
        !string.IsNullOrWhiteSpace(variant.Id)
        && !string.IsNullOrWhiteSpace(variant.Slug)
        && !string.IsNullOrWhiteSpace(variant.Header?.CategoryPath)
        && variant.Id.IsPostId();

    static void ResolveVariantIdentity(List<ParsedVariant> variants, GenerationReport report)
    {
        var identityByKey = new Dictionary<string, (string Id, string Slug)>(StringComparer.OrdinalIgnoreCase);

        foreach (var variant in variants)
        {
            if (string.IsNullOrWhiteSpace(variant.Header?.Id))
                continue;

            if (!variant.Header.Id.TryNormalizePostId(out var normalizedId))
            {
                report.Errors.Add($"Invalid post id in {variant.FilePath}.md (use digits only)");
                continue;
            }

            variant.Id = normalizedId;
            variant.Slug = variant.Header.Slug?.Trim();

            if (string.IsNullOrWhiteSpace(variant.Slug))
            {
                report.Errors.Add($"Missing slug in {variant.FilePath}.md");
                continue;
            }

            identityByKey[$"{variant.Directory}/{variant.BaseName}"] = (variant.Id, variant.Slug);
        }

        foreach (var variant in variants)
        {
            if (!string.IsNullOrWhiteSpace(variant.Id))
                continue;

            if (!identityByKey.TryGetValue($"{variant.Directory}/{variant.BaseName}", out var identity))
            {
                report.Errors.Add(
                    $"Missing id in {variant.FilePath}.md. Add id/slug front matter or provide an English sibling file.");
                continue;
            }

            variant.Id = identity.Id;
            variant.Slug = identity.Slug;
            report.Info.Add($"Linked {variant.FilePath}.md to post id '{identity.Id}' by file name.");
        }
    }

    static bool TryParseVariant(FileInfo file, DirectoryInfo root, out ParsedVariant variant, GenerationReport report)
    {
        variant = null!;

        string markdown;
        try
        {
            markdown = File.ReadAllText(file.FullName, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            report.Errors.Add($"Failed to read {file.Name}: {ex.Message}");
            return false;
        }

        var relativePath = Path.GetRelativePath(root.FullName, file.FullName).Replace('\\', '/');
        var filePath = Path.ChangeExtension(relativePath, null)!;
        var directory = Path.GetDirectoryName(relativePath)?.Replace('\\', '/') ?? "";
        var fileName = Path.GetFileNameWithoutExtension(file.Name);
        var (baseName, language) = fileName.ParseLanguageFromFileName();

        var header = markdown.ParsePostFrontMatter();
        if (header is null && language.Equals(PathExtensions.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            report.Errors.Add($"No front matter in {relativePath}");

        variant = new ParsedVariant
        {
            FilePath = filePath,
            Directory = directory,
            BaseName = baseName,
            Language = language,
            Header = header
        };

        return true;
    }

    static void SortPosts(List<PostIndex> posts) => posts.Sort(static (a, b) =>
    {
        var cmp = Nullable.Compare(b.Date, a.Date);
        return cmp != 0 ? cmp : PostIdExtensions.ComparePostIds(a.Id, b.Id);
    });
}

#endregion

#region Utilities

static class HashHelper
{
    public static string ComputeFileHash(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }
}

static class JsonWriter
{
    static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task WriteAsync<T>(string outputPath, T value)
    {
        var dir = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await using var stream = File.Create(outputPath);
        await JsonSerializer.SerializeAsync(stream, value, Options);
    }
}

static class JsonReader
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task<T?> ReadAsync<T>(string path)
    {
        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(stream, Options);
    }
}

#endregion
