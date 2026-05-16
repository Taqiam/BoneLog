namespace BoneLog.Models;

public class AboutMe
{
    public string Name { get; set; } = "";
    public string? Headline { get; set; }
    public string? Avatar { get; set; }
    public string Content { get; set; } = "";

    public static AboutMe Create(AboutMeFrontMatter? frontMatter, string htmlContent) => new()
    {
        Name = frontMatter?.Name ?? "",
        Headline = frontMatter?.Headline,
        Avatar = frontMatter?.Avatar,
        Content = htmlContent
    };
}


