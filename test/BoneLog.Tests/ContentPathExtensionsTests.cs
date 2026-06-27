namespace BoneLog.Tests;

public class ContentPathExtensionsTests
{
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
    [InlineData("/images/Logo.png", "images/Logo.png")]
    [InlineData("images/Logo.png", "images/Logo.png")]
    [InlineData("https://cdn.example.com/a.png", "https://cdn.example.com/a.png")]
    [InlineData("//cdn.example.com/a.png", "//cdn.example.com/a.png")]
    public void ResolveAssetUrl_RootAndAbsolutePaths(string input, string expected) =>
        Assert.Equal(expected, ContentPathExtensions.ResolveAssetUrl(input));

    [Fact]
    public void ResolveHtmlAssetUrls_RewritesRootImgSrc()
    {
        var html = """<p><img src="/images/pic.png" alt="pic"></p>""";
        var resolved = html.ResolveHtmlAssetUrls();

        Assert.Contains("src=\"images/pic.png\"", resolved);
    }

    [Fact]
    public void ResolveHtmlAssetUrls_PreservesSpaceBeforeSrcWhenFirstAttribute()
    {
        var html = """<p><img src="/images/Logo.png" alt="cover" /></p>""";
        var resolved = html.ResolveHtmlAssetUrls();

        Assert.Contains("<img src=\"images/Logo.png\"", resolved);
        Assert.DoesNotContain("<imgsrc=", resolved);
    }

    [Fact]
    public void ToPost_ResolvesCoverAndInlineImages()
    {
        var settings = new PathSettings("data/", "posts/", "index.json", "AboutMe.md");
        var markdown = """
            ---
            title: Test
            cover: /images/cover.jpg
            ---
            ![inline](/images/inline.jpg)
            """;

        var post = markdown.ToPost("Catt/FirstPost", settings);

        Assert.Equal("images/cover.jpg", post.Cover);
        Assert.Contains("<img src=\"images/inline.jpg\"", post.Content);
    }
}
