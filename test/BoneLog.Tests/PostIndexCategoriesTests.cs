namespace BoneLog.Tests;

public class PostIndexCategoriesTests
{
    [Fact]
    public void BuildFromIndex_EmptyPosts_ReturnsEmpty()
    {
        Assert.Empty(PostIndexCategoriesHelper.BuildCategory([]));
    }

    [Fact]
    public void BuildFromIndex_SingleCategory_CountsPost()
    {
        var posts = new[] { new PostIndex { Title = "A", Path = "a", Category = "Dev Journal" } };

        var tree = PostIndexCategoriesHelper.BuildCategory(posts);

        Assert.Single(tree);
        Assert.Equal("Dev Journal", tree[0].Title);
        Assert.Equal(1, tree[0].NumberOfPosts);
        Assert.Null(tree[0].SubCategories);
    }

    [Fact]
    public void BuildFromIndex_NestedPath_BuildsTreeWithTotals()
    {
        var posts = new[]
        {
            new PostIndex { Title = "A", Path = "a", Category = "Dev Journal / Tutorials" },
            new PostIndex { Title = "B", Path = "b", Category = "Dev Journal / News" },
            new PostIndex { Title = "C", Path = "c", Category = "Dev Journal / Tutorials" },
        };

        var tree = PostIndexCategoriesHelper.BuildCategory(posts);

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
