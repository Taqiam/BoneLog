namespace BoneLog.Tests;

public class PathSettingsTests
{
    [Theory]
    [InlineData("https://example.com/data/", "posts/", "https://example.com/data/posts/")]
    [InlineData("https://example.com/data/", "posts.json", "https://example.com/data/posts.json")]
    [InlineData("./data/", "AboutMe.md", "./data/AboutMe.md")]
    public void ResolvePath_CombinesRelativePathsWithBase(string basePath, string relative, string expected)
    {
        var settings = new PathSettings(basePath, relative, relative, relative);

        Assert.Equal(expected, settings.GetPostsPath());
    }

    [Theory]
    [InlineData("https://cdn.example.com/posts/")]
    [InlineData("http://localhost:7215/data/posts.json")]
    public void ResolvePath_ReturnsAbsoluteUrlsUnchanged(string absoluteUrl)
    {
        var settings = new PathSettings("https://example.com/data/", absoluteUrl, absoluteUrl, absoluteUrl);

        Assert.Equal(absoluteUrl, settings.GetPostsPath());
        Assert.Equal(absoluteUrl, settings.GetIndexPath());
        Assert.Equal(absoluteUrl, settings.GetAboutMePath());
    }
}
