using BoneLog.Tools;

namespace BoneLog.Models;

public class Post
{
    public string Id { get; set; } = "";
    public string Slug { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? ShortDescription { get; set; }
    public string? Category { get; set; }
    public string[]? Tags { get; set; }
    public DateTime? Date { get; set; }
    public string? Cover { get; set; }
    public string? Thumbnail { get; set; }
    public string Language { get; set; } = "en";

    public static Post Create(string filePath, string htmlContent, PostFrontMatter? frontMatter, string language)
    {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath.NormalizeRelativePath());
        var (baseName, _) = fileName.ParseLanguageFromFileName();
        var title = string.IsNullOrWhiteSpace(frontMatter?.Title) ? baseName.SlugToTitle() : frontMatter.Title;

        return new Post
        {
            FilePath = filePath,
            Id = frontMatter?.Id.NormalizePostIdOrNull() ?? "",
            Slug = frontMatter?.Slug ?? "",
            Content = htmlContent,
            Title = title,
            ShortDescription = frontMatter?.ShortDescription,
            Tags = frontMatter?.Tags,
            Date = PostFrontMatter.ParseDate(frontMatter?.Date),
            Cover = frontMatter?.Cover,
            Thumbnail = frontMatter?.Thumbnail,
            Category = frontMatter?.CategoryPath,
            Language = language
        };
    }
}
