namespace BoneLog.Models;

public class PostIndex
{
    public string Title { get; set; } = "";
    public string Path { get; set; } = "";
    public string? ShortDescription { get; set; }
    public string? Category { get; set; }
    public string[]? Tags { get; set; }
    public DateTime? Date { get; set; }
    public string? Thumbnail { get; set; }
}
