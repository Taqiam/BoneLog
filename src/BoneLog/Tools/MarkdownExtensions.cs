using BoneLog.Models;
using Markdig;
using System.Net;
using System.Text.RegularExpressions;

namespace BoneLog.Tools;

public static partial class MarkdownExtensions
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    [GeneratedRegex(@"^---\s*\n(.*?)\n---\s*\n(.*)$", RegexOptions.Singleline)]
    private static partial Regex FrontMatterRegex();

    [GeneratedRegex(@"<pre><code class=""language-mermaid"">(.*?)</code></pre>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex MermaidCodePreRegex();

    [GeneratedRegex(@"<pre\s+class=""mermaid"">(.*?)</pre>", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex MermaidClassPreRegex();

    public static string ToHtml(this string markdown) =>
        Markdown.ToHtml(markdown, Pipeline).ApplyMermaid().ApplyAutoDirection();

    public static string WithoutFrontMatter(this string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return markdown;

        var match = FrontMatterRegex().Match(markdown);
        return match.Success ? match.Groups[2].Value.TrimStart() : markdown;
    }

    public static (PostFrontMatter?, string Html) ToHtmlWithPostFrontMatter(this string markdown) =>
        markdown.ToHtmlWithFrontMatter<PostFrontMatter>();

    public static (T?, string Html) ToHtmlWithFrontMatter<T>(this string markdown) where T : class, new()
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return (null, markdown);

        var match = FrontMatterRegex().Match(markdown);
        if (!match.Success)
            return (null, markdown.ToHtml());

        var yamlContent = match.Groups[1].Value.Trim();
        var markdownBody = match.Groups[2].Value.Trim();
        var htmlBody = markdownBody.ToHtml();

        try
        {
            var header = yamlContent.DeserializeFrontMatter<T>();
            return (header, htmlBody);
        }
        catch
        {
            return (null, htmlBody);
        }
    }

    public static Post ToPost(this string markdown, string normalizedPath, PathSettings pathSettings)
    {
        var (frontMatter, htmlContent) = markdown.ToHtmlWithPostFrontMatter();
        htmlContent = htmlContent.ResolveHtmlAssetUrls(normalizedPath, pathSettings);

        var post = Post.Create(normalizedPath, htmlContent, frontMatter);
        post.Cover = ContentPathExtensions.ResolvePostAssetUrl(normalizedPath, post.Cover, pathSettings);
        post.Thumbnail = ContentPathExtensions.ResolvePostAssetUrl(normalizedPath, post.Thumbnail, pathSettings);
        return post;
    }

    public static AboutMe ToAboutMe(this string markdown, PathSettings pathSettings, string markdownRelativePath = "AboutMe.md")
    {
        var (frontMatter, htmlContent) = markdown.ToHtmlWithFrontMatter<AboutMeFrontMatter>();
        htmlContent = htmlContent.ResolveHtmlAssetUrls(markdownRelativePath, pathSettings, isPost: false);

        var about = AboutMe.Create(frontMatter, htmlContent);
        about.Avatar = ContentPathExtensions.ResolveDataFileAssetUrl(markdownRelativePath, about.Avatar, pathSettings);
        return about;
    }

    public static string ToHtmlBody(this string markdown, string contentRelativePath, PathSettings pathSettings, bool isPost = false) =>
        markdown.WithoutFrontMatter().ToHtml().ResolveHtmlAssetUrls(contentRelativePath, pathSettings, isPost);

    private static string ApplyMermaid(this string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        return html
            .ReplaceMermaidBlocks(MermaidCodePreRegex())
            .ReplaceMermaidBlocks(MermaidClassPreRegex());
    }

    private static string ReplaceMermaidBlocks(this string html, Regex pattern) =>
        pattern.Replace(html, match => match.Groups[1].Value.ToMermaidDiv());

    private static string ToMermaidDiv(this string source)
    {
        source = WebUtility.HtmlDecode(source);
        var encoded = WebUtility.HtmlEncode(source);
        return $"""<div class="mermaid" data-source="{encoded}">{encoded}</div>""";
    }

    private static string ApplyAutoDirection(this string html)
    {
        const string tags = "p|div|span|h1|h2|h3|h4|h5|h6";
        var regex = new Regex($@"<({tags})(?![^>]*dir=)([^>]*)>(\s*[\u0600-\u06FF])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return regex.Replace(html, @"<$1 dir=""rtl""$2>$3");
    }
}
