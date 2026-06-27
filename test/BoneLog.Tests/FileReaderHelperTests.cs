namespace BoneLog.Tests;

public class FileReaderHelperTests
{
    [Fact]
    public void ParseMarkdownToHtmlWithFrontMatter_ReturnsMetadataAndHtml()
    {
        string markdown = """
        ---
        title: Hello World
        date: "2025-05-31"
        tags: [test, example]
        cover: cover.jpg
        shortDescription: A test post
        ---
        # Welcome

        This is a test post.
        """;

        var (frontMatter, html) = markdown.ToHtmlWithPostFrontMatter();

        Assert.NotNull(frontMatter);
        Assert.Equal("Hello World", frontMatter!.Title);
        Assert.Equal("2025-05-31", frontMatter.Date);
        Assert.Equal(new[] { "test", "example" }, frontMatter.Tags);
        Assert.Equal("cover.jpg", frontMatter.Cover);
        Assert.Equal("A test post", frontMatter.ShortDescription);
        Assert.Contains("<h1 ", html);
        Assert.Contains("Welcome", html);
    }

    [Fact]
    public void Post_Create_MergesFrontMatterAndFilePath()
    {
        var frontMatter = new PostFrontMatter
        {
            Id = "123456",
            Slug = "hello-world",
            CategoryPath = "Tutorials",
            Title = "Hello World",
            Date = "2025-05-31",
            Tags = ["test"],
            ShortDescription = "Summary"
        };

        var post = Post.Create("tutorials/getting-started.en", "<p>Hi</p>", frontMatter, "en");

        Assert.Equal("Hello World", post.Title);
        Assert.Equal("tutorials/getting-started.en", post.FilePath);
        Assert.Equal("123456", post.Id);
        Assert.Equal("hello-world", post.Slug);
        Assert.Equal("<p>Hi</p>", post.Content);
        Assert.Equal("Tutorials", post.Category);
        Assert.Equal("Summary", post.ShortDescription);
        Assert.Equal(new[] { "test" }, post.Tags);
        Assert.Equal(new DateTime(2025, 5, 31), post.Date?.Date);
        Assert.Equal("en", post.Language);
    }

    [Fact]
    public void Post_Create_UsesCategoryPathFromFrontMatter()
    {
        var frontMatter = new PostFrontMatter
        {
            Id = "001",
            Slug = "my-post",
            CategoryPath = "Dev Journal"
        };

        var post = Post.Create("dev-journal/my-post.en", "<p>Hi</p>", frontMatter, "en");

        Assert.Equal("Dev Journal", post.Category);
    }

    [Fact]
    public void RemoveYamlHeader_WithValidFrontMatter_RemovesYaml()
    {
        string markdown = """
                          ---
                          title: Hello
                          ---
                          # Hello World
                          """;

        var result = markdown.WithoutFrontMatter();

        Assert.DoesNotContain("---", result);
        Assert.Contains("# Hello World", result);
    }

    [Fact]
    public void RemoveYamlHeader_WithoutFrontMatter_ReturnsOriginal()
    {
        string markdown = "# Hello World";

        var result = markdown.WithoutFrontMatter();

        Assert.Equal(markdown, result);
    }

    [Fact]
    public void MarkdownToHtml_WithSimpleMarkdown_ReturnsExpectedHtml()
    {
        string markdown = "# Heading";

        var html = markdown.ToHtml();

        Assert.Contains("<h1", html);
        Assert.Contains("Heading", html);
    }

    [Fact]
    public void MarkdownToHtml_WithMermaidBlock_EmitsMermaidDiv()
    {
        var markdown = """
            ```mermaid
            flowchart LR
              A --> B
            ```
            """;

        var html = markdown.ToHtml();

        Assert.Contains("<div class=\"mermaid\"", html);
        Assert.Contains("data-source=", html);
        Assert.Contains("flowchart LR", html);
        Assert.DoesNotContain("<pre class=\"mermaid\"", html);
    }

    [Fact]
    public void MarkdownToHtml_WithRTLText_AddsDirRtl()
    {
        var markdown = "سلام دنیا";

        var html = markdown.ToHtml();

        Assert.Contains(@"dir=""rtl""", html);
    }

    [Fact]
    public void ParseMarkdownToHtmlWithHeader_WithoutFrontMatter_ReturnsNullMetadata()
    {
        var markdown = "# No Header";

        var (meta, html) = markdown.ToHtmlWithFrontMatter<PostFrontMatter>();

        Assert.Null(meta);
        Assert.Contains("<h1", html);
    }

    [Fact]
    public void ParseMarkdownToHtmlWithHeader_WithInvalidYaml_ReturnsNullHeader()
    {
        var markdown = """
                          ---
                          title Hello World
                          ---
                          # Title
                          """;

        var (meta, html) = markdown.ToHtmlWithFrontMatter<PostFrontMatter>();

        Assert.Null(meta);
        Assert.Contains("Title", html);
    }
}
