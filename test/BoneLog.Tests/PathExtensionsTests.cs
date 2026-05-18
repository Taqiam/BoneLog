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

    [Fact]
    public void CategoryFromPath_JoinsParentFolders() =>
        Assert.Equal("Dev Journal", "dev-journal/my-post".CategoryFromPath());
}
