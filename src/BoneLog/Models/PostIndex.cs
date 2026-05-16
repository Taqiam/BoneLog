namespace BoneLog.Models;

public record class PostIndex(string Title, string Path, string? Content = null)
{
    public string Title { get; set; } = Title;
    public string Path { get; set; } = Path;
    public string? ShortDescription { get; set; }

    public string? Category { get; set; }
    public string[]? Tags { get; set; }

    public DateTime? Date { get; set; }

    public string? Thumbnail { get; set; }
}