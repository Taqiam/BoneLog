namespace BoneLog.Models;

public class Category
{
    public string Title { get; set; } = null!;
    public int NumberOfPosts { get; set; }
    public Category[]? SubCategories { get; set; }
}