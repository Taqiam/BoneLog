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

    public static Post Create(string path, string htmlContent, PostFrontMatter? frontMatter, string? category = null)
    {
        var title = string.IsNullOrWhiteSpace(frontMatter?.Title) ? PostPathHelper.FolderNameToTitle(System.IO.Path.GetFileNameWithoutExtension(path.Replace('\\', '/'))) : frontMatter.Title;

        return new Post
        {
            Path = path,
            Content = htmlContent,
            Title = title,
            ShortDescription = frontMatter?.ShortDescription,
            Tags = frontMatter?.Tags,
            Date = frontMatter?.Date.ToDateTiem(),
            Cover = frontMatter?.Cover,
            Thumbnail = frontMatter?.Thumbnail,
            Category = category ?? PostPathHelper.CategoryFromPath(path)
        };
    }
}