namespace BoneLog.Tests;

public class ContentPathExtensionsTests
{
    private static readonly PathSettings Settings = new(
        "https://example.com/data/",
        "posts/",
        "index.json",
        "AboutMe.md");

    [Theory]
    [InlineData(null, "/")]
    [InlineData("", "/")]
    [InlineData("/", "/")]
    [InlineData("myblog", "/myblog/")]
    [InlineData("/myblog", "/myblog/")]
    [InlineData("/myblog/", "/myblog/")]
    public void NormalizeBaseDir_FormatsApplicationBase(string? input, string expected) =>
        Assert.Equal(expected, ContentPathExtensions.NormalizeBaseDir(input));

    [Theory]
    [InlineData("https://cdn.example.com/a.png", true)]
    [InlineData("/images/logo.png", true)]
    [InlineData("images/logo.png", false)]
    public void ShouldKeepAssetPathAsIs_DetectsAbsolutePaths(string path, bool expected) =>
        Assert.Equal(expected, ContentPathExtensions.ShouldKeepAssetPathAsIs(path));

    [Fact]
    public void ResolvePostAssetUrl_ResolvesRelativeToPostDirectory()
    {
        var url = ContentPathExtensions.ResolvePostAssetUrl("Catt/FirstPost", "images/Logo.jpg", Settings);

        Assert.Equal("https://example.com/data/posts/Catt/images/Logo.jpg", url);
    }

    [Fact]
    public void ResolvePostAssetUrl_ResolvesParentSegmentsToSiteImages()
    {
        var url = ContentPathExtensions.ResolvePostAssetUrl(
            "Catt/FirstPost",
            "../../../images/Logo.jpg",
            Settings);

        Assert.Equal("https://example.com/images/Logo.jpg", url);
    }

    [Fact]
    public void ResolvePostAssetUrl_ResolvesSecondPostCoverToSiteImages()
    {
        var url = ContentPathExtensions.ResolvePostAssetUrl(
            "SecondPost",
            "../../images/Logo.jpg",
            Settings);

        Assert.Equal("https://example.com/images/Logo.jpg", url);
    }

    [Fact]
    public void ResolvePostAssetUrl_KeepsAbsoluteUrl()
    {
        const string absolute = "https://cdn.example.com/photo.jpg";
        var url = ContentPathExtensions.ResolvePostAssetUrl("Catt/FirstPost", absolute, Settings);

        Assert.Equal(absolute, url);
    }

    [Fact]
    public void ResolveHtmlAssetUrls_RewritesImgSrcRelativeToPost()
    {
        var html = """<p><img src="images/pic.png" alt="pic"></p>""";
        var resolved = html.ResolveHtmlAssetUrls("Catt/FirstPost", Settings);

        Assert.Contains("src=\"https://example.com/data/posts/Catt/images/pic.png\"", resolved);
    }

    [Fact]
    public void ToPost_ResolvesCoverAndInlineImages()
    {
        var markdown = """
            ---
            title: Test
            cover: images/cover.jpg
            ---
            ![inline](images/inline.jpg)
            """;

        var post = markdown.ToPost("Catt/FirstPost", Settings);

        Assert.Equal("https://example.com/data/posts/Catt/images/cover.jpg", post.Cover);
        Assert.Contains("https://example.com/data/posts/Catt/images/inline.jpg", post.Content);
    }
}
