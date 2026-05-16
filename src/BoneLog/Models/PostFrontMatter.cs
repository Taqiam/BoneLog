namespace BoneLog.Models;

public class PostFrontMatter
{
    public string? Title { get; set; }
    public string? Date { get; set; }
    public string[]? Tags { get; set; }
    public string? Cover { get; set; }
    public string? Thumbnail { get; set; }
    public string? ShortDescription { get; set; }
    public string? Language { get; set; }
}
