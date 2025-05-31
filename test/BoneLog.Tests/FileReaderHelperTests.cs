namespace BoneLog.Tests;

using System.Collections.Generic;
using Blazor.Dtos;
using Blazor.Utilites;
using Xunit;

public class FileReaderHelperTests
{

    [Fact]
    public void ParseMarkdownToHtmlWithHeader_ReturnsMetadataAndHtml()
    {
        // arrange
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

        // act
        var (post, html) = markdown.ParseMarkdownToHtmlWithHeader<PostHeaderDto>();

        // assert
        Assert.NotNull(post);
        Assert.Equal("Hello World", post!.Title);
        Assert.Equal("31-05-2025", post.Date);
        Assert.Equal(new List<string> { "test", "example" }, post.Tags);
        Assert.Contains("<h1 ", html);
        Assert.Contains("Welcome", html);
    }

}
