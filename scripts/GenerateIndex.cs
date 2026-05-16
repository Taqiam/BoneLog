#:package YamlDotNet@16.3.0
#:property JsonSerializerIsReflectionEnabledByDefault=true

using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

#region Entry

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: dotnet run GenerateIndex.cs -- <rootPath> <index.json> <categories.json>");
    Environment.Exit(1);
}

var rootPath = Path.GetFullPath(args[0]);
var indexOut = Path.GetFullPath(args[1]);
var categoriesOut = Path.GetFullPath(args[2]);

if (!Directory.Exists(rootPath))
{
    Console.Error.WriteLine($"Root path not found: {rootPath}");
    Environment.Exit(1);
}

var posts = IndexBuilder.Build(rootPath);
var categories = CategoryBuilder.Build(rootPath);

await JsonWriter.WriteAsync(indexOut, posts);
await JsonWriter.WriteAsync(categoriesOut, categories);

Console.WriteLine($"Wrote {posts.Count} posts -> {indexOut}");
Console.WriteLine($"Wrote {categories.Length} top-level categories -> {categoriesOut}");

#endregion

#region Models

sealed class Category
{
    public string Title { get; set; } = "";
    public int NumberOfPosts { get; set; }
    public Category[]? SubCategories { get; set; }
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
}

sealed class PostFrontMatter
{
    public string? Title { get; set; }
    public string? Date { get; set; }
    public string[]? Tags { get; set; }
    public string? Thumbnail { get; set; }
    public string? ShortDescription { get; set; }
}

#endregion

#region IndexBuilder

static class IndexBuilder
{
    static readonly Regex FrontMatter = new(
        @"^---\s*\n(.*?)\n---\s*\n(.*)$",
        RegexOptions.Singleline | RegexOptions.Compiled);

    static readonly IDeserializer Yaml = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public static List<PostIndex> Build(string rootPath)
    {
        var root = new DirectoryInfo(rootPath);
        var posts = new List<PostIndex>();

        foreach (var file in root.EnumerateFiles("*.md", SearchOption.AllDirectories))
        {
            if (TryParsePost(file, root, out var post))
                posts.Add(post);
        }

        posts.Sort(static (a, b) =>
        {
            var cmp = Nullable.Compare(b.Date, a.Date);
            return cmp != 0 ? cmp : string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase);
        });

        return posts;
    }

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

        var relativePath = Path.GetRelativePath(root.FullName, file.FullName)
            .Replace('\\', '/');
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

        var parentDir = file.Directory;
        var categoryFromFolder = parentDir is not null && !IsSamePath(parentDir.FullName, root.FullName)
            ? FolderNames.ToTitle(parentDir.Name)
            : null;

        post.Title = string.IsNullOrWhiteSpace(header?.Title)
            ? FolderNames.ToTitle(Path.GetFileNameWithoutExtension(file.Name))
            : header.Title;
        post.Path = postPath;
        post.ShortDescription = header?.ShortDescription;
        post.Tags = header?.Tags;
        post.Thumbnail = header?.Thumbnail;
        post.Category = categoryFromFolder;
        post.Date = ParseDate(header?.Date);

        return true;
    }

    static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt)
            ? dt
            : null;
    }

    static bool IsSamePath(string a, string b) =>
        string.Equals(Path.GetFullPath(a), Path.GetFullPath(b), StringComparison.OrdinalIgnoreCase);
}

#endregion

#region CategoryBuilder

static class CategoryBuilder
{
    public static Category[] Build(string rootPath)
    {
        var root = new DirectoryInfo(rootPath);
        var categories = new List<Category>();

        foreach (var dir in root.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
        {
            var node = BuildNode(dir);
            if (node.NumberOfPosts > 0)
                categories.Add(node);
        }

        return categories.ToArray();
    }

    static Category BuildNode(DirectoryInfo dir)
    {
        var directPosts = dir.EnumerateFiles("*.md", SearchOption.TopDirectoryOnly).Count();
        var subCategories = new List<Category>();

        foreach (var child in dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
        {
            var sub = BuildNode(child);
            if (sub.NumberOfPosts > 0)
                subCategories.Add(sub);
        }

        var total = directPosts + subCategories.Sum(c => c.NumberOfPosts);

        return new Category
        {
            Title = FolderNames.ToTitle(dir.Name),
            NumberOfPosts = total,
            SubCategories = subCategories.Count > 0 ? subCategories.ToArray() : null
        };
    }
}

#endregion

#region Utilities

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

#endregion
