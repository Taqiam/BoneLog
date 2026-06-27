namespace BoneLog.Models;

public class PostIndex
{
    public string Id { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string? ShortDescription { get; set; }
    public string? Category { get; set; }
    public string[]? Tags { get; set; }
    public DateTime? Date { get; set; }
    public string? Thumbnail { get; set; }
    public string[] Languages { get; set; } = [];
    public Dictionary<string, string>? LocalizedTitles { get; set; }
    public Dictionary<string, string> FilePaths { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}
