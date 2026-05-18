namespace BoneLog.Tests;

public class PostIndexHelpersTests
{
    [Fact]
    public void GetLanguages_GroupsAndOrdersByName()
    {
        var posts = new[]
        {
            new PostIndex { Title = "A", Path = "a", Language = "EN" },
            new PostIndex { Title = "B", Path = "b", Language = "FA" },
            new PostIndex { Title = "C", Path = "c", Language = "EN" },
        };

        var languages = posts.GetLanguages();

        Assert.Equal(2, languages.Length);
        Assert.Equal("EN", languages[0].Language);
        Assert.Equal(2, languages[0].Count);
        Assert.Equal("FA", languages[1].Language);
        Assert.Equal(1, languages[1].Count);
    }

    [Fact]
    public void GetTags_CountsDistinctTagsPerPost()
    {
        var posts = new[]
        {
            new PostIndex { Title = "A", Path = "a", Tags = ["dotnet", "blazor"] },
            new PostIndex { Title = "B", Path = "b", Tags = ["dotnet"] },
            new PostIndex { Title = "C", Path = "c", Tags = ["dotnet", "dotnet"] },
        };

        var tags = posts.GetTags();

        Assert.Equal(2, tags.Length);
        Assert.Equal("blazor", tags[0].Tag);
        Assert.Equal(1, tags[0].Count);
        Assert.Equal("dotnet", tags[1].Tag);
        Assert.Equal(3, tags[1].Count);
    }
}
