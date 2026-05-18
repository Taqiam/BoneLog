using BoneLog.Tools;

namespace BoneLog.Models;

public class Post
{
    public string Title { get; set; } = "";
    public string Path { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ShortDescription { get; set; }
    public string? Category { get; set; }
    public string[]? Tags { get; set; }
    public DateTime? Date { get; set; }
    public string? Cover { get; set; }
    public string? Thumbnail { get; set; }
    public string Language { get; set; } = "EN";

    public static Post Create(string path, string htmlContent, PostFrontMatter? frontMatter, string? category = null)
    {
        var title = string.IsNullOrWhiteSpace(frontMatter?.Title) ? System.IO.Path.GetFileNameWithoutExtension(path.NormalizeRelativePath()).SlugToTitle() : frontMatter.Title;

        return new Post
        {
            Path = path,
            Content = htmlContent,
            Title = title,
            ShortDescription = frontMatter?.ShortDescription,
            Tags = frontMatter?.Tags,
            Date = frontMatter?.Date.ToDateTime(),
            Cover = frontMatter?.Cover,
            Thumbnail = frontMatter?.Thumbnail,
            Category = category ?? path.CategoryFromPath(),
            Language = string.IsNullOrWhiteSpace(frontMatter?.Language) ? "EN" : frontMatter.Language.Trim()
        };
    }
}