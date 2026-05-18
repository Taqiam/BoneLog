#:package YamlDotNet@16.3.0
#:property JsonSerializerIsReflectionEnabledByDefault=true

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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

if (!Directory.Exists(rootPath))
{
    Console.Error.WriteLine($"Root path not found: {rootPath}");
    Environment.Exit(1);
}

var result = options.Full
    ? IndexBuilder.BuildFull(rootPath)
    : await IndexBuilder.BuildIncrementalAsync(rootPath, indexOut, manifestOut);

await JsonWriter.WriteAsync(indexOut, result.Posts);
await JsonWriter.WriteAsync(manifestOut, result.Manifest);

Console.WriteLine(
    options.Full
        ? $"Full build: wrote {result.Posts.Count} posts -> {indexOut}"
        : $"Updated {result.UpdatedCount}, kept {result.KeptCount}, removed {result.RemovedCount} -> {indexOut}");

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
}

sealed class PostIndex
{
    public string Title { get; set; } = "";
    public string Path { get; set; } = "";
    public string? Content { get; set; }
    public string? ShortDescription { get; set; }
    public string? Category { get; set; }
    public string[]? Tags { get; set; }
    public DateTime? Date { get; set; }
    public string? Thumbnail { get; set; }
    public string Language { get; set; } = "EN";
}
sealed class PostFrontMatter
{
    public string? Title { get; set; }
    public string? Date { get; set; }
    public string[]? Tags { get; set; }
    public string? Thumbnail { get; set; }
    public string? ShortDescription { get; set; }
    public string? Language { get; set; }
}

#endregion

#region IndexBuilder

static class IndexBuilder
{
    static readonly Regex FrontMatter = new(@"^---\s*\n(.*?)\n---\s*\n(.*)$", RegexOptions.Singleline | RegexOptions.Compiled);

    static readonly IDeserializer Yaml = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public static BuildResult BuildFull(string rootPath)
    {
        var root = new DirectoryInfo(rootPath);
        var posts = new List<PostIndex>();
        var manifest = new IndexManifest();

        foreach (var file in root.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            if (!TryParsePost(file, root, out var post))
                continue;

            posts.Add(post);
            manifest.Hashes[post.Path] = HashHelper.ComputeFileHash(file.FullName);
        }

        SortPosts(posts);
        return new BuildResult
        {
            Posts = posts,
            Manifest = manifest,
            UpdatedCount = posts.Count,
            KeptCount = 0,
            RemovedCount = 0
        };
    }

    public static async Task<BuildResult> BuildIncrementalAsync(string rootPath, string indexPath, string manifestPath)
    {
        if (!File.Exists(indexPath) || !File.Exists(manifestPath))
            return BuildFull(rootPath);

        var existingPosts = await JsonReader.ReadAsync<PostIndex[]>(indexPath) ?? [];
        var existingManifest = await JsonReader.ReadAsync<IndexManifest>(manifestPath) ?? new IndexManifest();

        var postsByPath = existingPosts.ToDictionary(p => p.Path, StringComparer.OrdinalIgnoreCase);
        var newHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var root = new DirectoryInfo(rootPath);

        var updated = 0;
        var kept = 0;

        foreach (var file in root.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(root.FullName, file.FullName).Replace('\\', '/');
            var postPath = Path.ChangeExtension(relativePath, null)!;
            var hash = HashHelper.ComputeFileHash(file.FullName);
            newHashes[postPath] = hash;

            if (existingManifest.Hashes.TryGetValue(postPath, out var previousHash) &&
                string.Equals(previousHash, hash, StringComparison.OrdinalIgnoreCase) &&
                postsByPath.TryGetValue(postPath, out var existing))
            {
                kept++;
                continue;
            }

            if (TryParsePost(file, root, out var post))
            {
                postsByPath[postPath] = post;
                updated++;
            }
            else
            {
                postsByPath.Remove(postPath);
            }
        }

        var removed = 0;
        foreach (var stalePath in postsByPath.Keys.Except(newHashes.Keys, StringComparer.OrdinalIgnoreCase).ToList())
        {
            postsByPath.Remove(stalePath);
            removed++;
        }

        var posts = postsByPath.Values.ToList();
        SortPosts(posts);

        return new BuildResult
        {
            Posts = posts,
            Manifest = new IndexManifest { Hashes = newHashes },
            UpdatedCount = updated,
            KeptCount = kept,
            RemovedCount = removed
        };
    }

    static void SortPosts(List<PostIndex> posts) => posts.Sort(static (a, b) =>
    {
        var cmp = Nullable.Compare(b.Date, a.Date);
        return cmp != 0 ? cmp : string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
    });

    static bool TryParsePost(FileInfo file, DirectoryInfo root, out PostIndex post)
    {
        post = new PostIndex();

        string markdown;
        try
        {
            markdown = File.ReadAllText(file.FullName, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to read {file.Name}: {ex.Message}");
            return false;
        }

        var relativePath = Path.GetRelativePath(root.FullName, file.FullName).Replace('\\', '/');
        var postPath = Path.ChangeExtension(relativePath, null)!;

        PostFrontMatter? header = null;
        var match = FrontMatter.Match(markdown);
        if (match.Success)
        {
            try
            {
                header = Yaml.Deserialize<PostFrontMatter>(match.Groups[1].Value.Trim());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Invalid YAML in {relativePath}: {ex.Message}");
            }
        }
        else
        {
            Console.Error.WriteLine($"No front matter in {relativePath}");
        }

        post.Title = string.IsNullOrWhiteSpace(header?.Title) ? FolderNames.ToTitle(Path.GetFileNameWithoutExtension(file.Name)) : header.Title;
        post.Path = postPath;
        post.ShortDescription = header?.ShortDescription;
        post.Tags = header?.Tags;
        post.Thumbnail = header?.Thumbnail;
        post.Category = CategoryPathFromPostPath(postPath);
        post.Date = ParseDate(header?.Date);
        post.Language = string.IsNullOrWhiteSpace(header?.Language) ? "EN" : header.Language.Trim();

        return true;
    }

    static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt) ? dt : null;
    }

    static string? CategoryPathFromPostPath(string postPath)
    {
        var parts = postPath.Replace('\\', '/').Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1) return null;

        var folderSegments = parts[..^1];
        return folderSegments.Length == 0 ? null : string.Join(" / ", folderSegments.Select(FolderNames.ToTitle));
    }
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

static class FolderNames
{
    public static string ToTitle(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return folderName;

        var words = folderName.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (words.Length == 0)
            return folderName;

        return string.Join(' ', words.Select(static w =>
            char.ToUpperInvariant(w[0]) + (w.Length > 1 ? w[1..].ToLowerInvariant() : "")));
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
