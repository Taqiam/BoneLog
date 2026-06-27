namespace BoneLog.Tests;

public class PathExtensionsTests
{
    [Theory]
    [InlineData("/cat/post", "cat/post")]
    [InlineData(@"cat\post", "cat/post")]
    [InlineData("cat/post", "cat/post")]
    public void NormalizeRelativePath_TrimsAndUnifiesSeparators(string input, string expected) =>
        Assert.Equal(expected, input.NormalizeRelativePath());

    [Theory]
    [InlineData("https://example.com/data/", "cat/post", "https://example.com/data/cat/post.md")]
    [InlineData("https://example.com/data", "/cat/post", "https://example.com/data/cat/post.md")]
    public void ToMarkdownFetchPath_BuildsFetchUrl(string basePath, string relative, string expected) =>
        Assert.Equal(expected, basePath.ToMarkdownFetchPath(relative));

    [Theory]
    [InlineData("dev-journal", "Dev Journal")]
    [InlineData("getting-started", "Getting Started")]
    public void SlugToTitle_FormatsKebabCase(string slug, string expected) =>
        Assert.Equal(expected, slug.SlugToTitle());

    [Theory]
    [InlineData("https://github.com/x", "https://github.com/x")]
    [InlineData("mailto:a@b.c", "mailto:a@b.c")]
    [InlineData("/about", "about")]
    [InlineData("posts/en/1/x", "posts/en/1/x")]
    [InlineData("#section", "#section")]
    public void ToAppRelativeUrl_KeepsAbsoluteAndStripsLeadingSlash(string input, string expected) =>
        Assert.Equal(expected, input.ToAppRelativeUrl());

    [Theory]
    [InlineData("https://x.com", true)]
    [InlineData("http://x.com", true)]
    [InlineData("//cdn/x", true)]
    [InlineData("about", false)]
    [InlineData("/about", false)]
    public void IsAbsoluteWebUrl_DetectsFullUrls(string input, bool expected) =>
        Assert.Equal(expected, input.IsAbsoluteWebUrl());

    [Theory]
    [InlineData("post.en", "post", "en")]
    [InlineData("post.fa", "post", "fa")]
    [InlineData("Writing-Posts", "Writing-Posts", "en")]
    public void ParseLanguageFromFileName_DetectsLanguageSuffix(string fileName, string baseName, string language)
    {
        var parsed = fileName.ParseLanguageFromFileName();
        Assert.Equal(baseName, parsed.BaseName);
        Assert.Equal(language, parsed.Language);
    }
}
