namespace BoneLog.Models;

public class Category
{
    public string Title { get; set; } = null!;
    public Category[]? SubCategories { get; set; }
}