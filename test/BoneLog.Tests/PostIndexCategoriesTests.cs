namespace BoneLog.Tests;

public class PostIndexCategoriesTests
{
    [Fact]
    public void GetCategories_EmptyPosts_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<PostIndex>().GetCategories());
    }

    [Fact]
    public void GetCategories_SingleCategory_CountsPost()
    {
        var posts = new[] { new PostIndex { Title = "A", Id = "1", Slug = "a", Languages = ["en"], FilePaths = new() { ["en"] = "a.en" }, Category = "Dev Journal" } };

        var tree = posts.GetCategories();

        Assert.Single(tree);
        Assert.Equal("Dev Journal", tree[0].Title);
        Assert.Equal(1, tree[0].NumberOfPosts);
        Assert.Null(tree[0].SubCategories);
    }

    [Fact]
    public void GetCategories_NestedPath_BuildsTreeWithTotals()
    {
        var posts = new[]
        {
            new PostIndex { Title = "A", Id = "1", Slug = "a", Languages = ["en"], FilePaths = new() { ["en"] = "a.en" }, Category = "Dev Journal / Tutorials" },
            new PostIndex { Title = "B", Id = "2", Slug = "b", Languages = ["en"], FilePaths = new() { ["en"] = "b.en" }, Category = "Dev Journal / News" },
            new PostIndex { Title = "C", Id = "3", Slug = "c", Languages = ["en"], FilePaths = new() { ["en"] = "c.en" }, Category = "Dev Journal / Tutorials" },
        };

        var tree = posts.GetCategories();

        Assert.Single(tree);
        var root = tree[0];
        Assert.Equal("Dev Journal", root.Title);
        Assert.Equal(3, root.NumberOfPosts);
        Assert.NotNull(root.SubCategories);
        Assert.Equal(2, root.SubCategories!.Length);

        var tutorials = Array.Find(root.SubCategories, c => c.Title == "Tutorials");
        Assert.NotNull(tutorials);
        Assert.Equal(2, tutorials!.NumberOfPosts);
    }
}
