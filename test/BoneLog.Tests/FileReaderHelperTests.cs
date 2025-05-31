namespace BoneLog.Tests;

using BoneLog.Blazor.Dtos;

public class FileReaderHelperTests
{

    [Fact]
    public void ParseMarkdownToHtmlWithHeader_ReturnsMetadataAndHtml()
    {
        // Arrange
        string markdown = """
        ---
        title: Hello World
        date: 31-05-2025
        tags: [test, example]
        cover: cover.jpg
        ---
        # Welcome

        This is a test post.
        """;

        // Act
        var (post, html) = FileReaderHelper.ParseMarkdownToHtmlWithHeader<PostMetadata>(markdown);

        // Assert
        Assert.NotNull(meta);
        Assert.Equal("Hello World", post!.Title);
        Assert.Equal("31-05-2025", post.Date);
        Assert.Equal(new List<string> { "test", "example" }, post.Tags);
        Assert.Contains("<h1>", html);
        Assert.Contains("Welcome", html);
    }

}
