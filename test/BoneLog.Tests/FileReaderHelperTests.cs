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
    
    [Fact]
    public void RemoveYamlHeader_WithValidFrontMatter_RemovesYaml()
    {
        // arrange
        string markdown = """
                          ---
                          title: Hello
                          ---
                          # Hello World
                          """;

        // act
        var result = markdown.RemoveYamlHeader();

        // assert
        Assert.DoesNotContain("---", result);
        Assert.Contains("# Hello World", result);
    }

    [Fact]
    public void RemoveYamlHeader_WithoutFrontMatter_ReturnsOriginal()
    {
        // arrange
        string markdown = "# Hello World";

        // act
        var result = markdown.RemoveYamlHeader();

        // assert
        Assert.Equal(markdown, result);
    }

    [Fact]
    public void MarkdownToHtml_WithSimpleMarkdown_ReturnsExpectedHtml()
    {
        // arrange
        string markdown = "# Heading";

        // act
        var html = markdown.MarkdownToHtml();

        // assert
        Assert.Contains("<h1", html);
        Assert.Contains("Heading", html);
    }

    [Fact]
    public void MarkdownToHtml_WithRTLText_AddsDirRtl()
    {
        // arrange
        var markdown = "سلام دنیا";

        // act
        var html = markdown.MarkdownToHtml();

        // assert
        Assert.Contains(@"dir=""rtl""", html);
    }
    
    [Fact]
    public void ParseMarkdownToHtmlWithHeader_WithoutFrontMatter_ReturnsNullMetadata()
    {
        // arrange
        var markdown = "# No Header";

        // act
        var (meta, html) = markdown.ParseMarkdownToHtmlWithHeader<PostHeaderDto>();

        // assert
        Assert.Null(meta);
        Assert.Contains("<h1", html);
    }
    
    [Fact]
    public void ParseMarkdownToHtmlWithHeader_WithInvalidYaml_ReturnsNullHeader()
    {
        // arrange
        var markdown = """
                          ---
                          title Hello World  // missing colon
                          ---
                          # Title
                          """;

        // act
        var (meta, html) = markdown.ParseMarkdownToHtmlWithHeader<PostHeaderDto>();

        // assert
        Assert.Null(meta);
        Assert.Contains("Title", html);
    }
}
