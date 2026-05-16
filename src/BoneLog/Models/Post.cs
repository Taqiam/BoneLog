namespace BoneLog.Models;

public record class Post(string Title, string Path, string Content)
{
    public Post() : this(string.Empty, string.Empty, string.Empty) { }

    public string Title { get; set; } = Title;
    public string Path { get; set; } = Path;
    public string Content { get; set; } = Content;
    public string? ShortDescription { get; set; }
    public string[]? Tags { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string? Cover { get; set; }
    public string? Thumbnail { get; set; }
}