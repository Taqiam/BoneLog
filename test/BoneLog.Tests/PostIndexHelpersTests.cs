namespace BoneLog.Tests;

public class PostIndexHelpersTests
{
    private static PostIndex CreatePost(
        string id,
        string slug,
        string title,
        params string[] languages)
    {
        var filePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var language in languages)
            filePaths[language] = $"{slug}.{language}";

        return new PostIndex
        {
            Id = id,
            Slug = slug,
            Title = title,
            Languages = languages,
            FilePaths = filePaths
        };
    }

    [Fact]
    public void GetLanguages_GroupsAndOrdersByName()
    {
        var posts = new[]
        {
            CreatePost("1", "a", "A", "en"),
            CreatePost("2", "b", "B", "fa"),
            CreatePost("3", "c", "C", "en"),
        };

        var languages = posts.GetLanguages();

        Assert.Equal(2, languages.Length);
        Assert.Equal("en", languages[0].Language);
        Assert.Equal(2, languages[0].Count);
        Assert.Equal("fa", languages[1].Language);
        Assert.Equal(1, languages[1].Count);
    }

    [Fact]
    public void GetTags_CountsDistinctTagsPerPost()
    {
        var posts = new[]
        {
            new PostIndex { Title = "A", Id = "1", Slug = "a", Languages = ["en"], FilePaths = new() { ["en"] = "a.en" }, Tags = ["dotnet", "blazor"] },
            new PostIndex { Title = "B", Id = "2", Slug = "b", Languages = ["en"], FilePaths = new() { ["en"] = "b.en" }, Tags = ["dotnet"] },
            new PostIndex { Title = "C", Id = "3", Slug = "c", Languages = ["en"], FilePaths = new() { ["en"] = "c.en" }, Tags = ["dotnet", "dotnet"] },
        };

        var tags = posts.GetTags();

        Assert.Equal(2, tags.Length);
        Assert.Equal("blazor", tags[0].Tag);
        Assert.Equal(1, tags[0].Count);
        Assert.Equal("dotnet", tags[1].Tag);
        Assert.Equal(3, tags[1].Count);
    }

    [Fact]
    public void GetDisplayTitle_UsesEnglishForAllAndEnglishFilters()
    {
        var post = new PostIndex
        {
            Id = "1",
            Slug = "hello",
            Title = "English title",
            LocalizedTitles = new() { ["fa"] = "Persian title" },
            Languages = ["en", "fa"],
            FilePaths = new() { ["en"] = "hello.en", ["fa"] = "hello.fa" }
        };

        Assert.Equal("English title", post.GetDisplayTitle(null));
        Assert.Equal("English title", post.GetDisplayTitle("all"));
        Assert.Equal("English title", post.GetDisplayTitle("en"));
        Assert.Equal("Persian title", post.GetDisplayTitle("fa"));
    }

    [Fact]
    public void GetDisplayTitle_FallsBackToEnglishWhenLocalizedMissing()
    {
        var post = new PostIndex
        {
            Id = "1",
            Slug = "hello",
            Title = "English title",
            Languages = ["en", "fa"],
            FilePaths = new() { ["en"] = "hello.en", ["fa"] = "hello.fa" }
        };

        Assert.Equal("English title", post.GetDisplayTitle("fa"));
    }

    [Fact]
    public void FilterByLanguage_ReturnsPostsWithRequestedLanguage()
    {
        var posts = new[]
        {
            CreatePost("1", "a", "A", "en", "fa"),
            CreatePost("2", "b", "B", "en"),
            CreatePost("3", "c", "C", "fa"),
        };

        var filtered = posts.FilterByLanguage("fa");

        Assert.Equal(2, filtered.Length);
        Assert.Contains(filtered, p => p.Id == "1");
        Assert.Contains(filtered, p => p.Id == "3");
    }
}
