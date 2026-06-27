using BoneLog.Models;
using Markdig;
using System.Net;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BoneLog.Tools;

public static partial class MarkdownExtensions
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    private static readonly IDeserializer FrontMatterDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

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

    public static PostFrontMatter? ParsePostFrontMatter(this string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return null;

        var match = FrontMatterRegex().Match(markdown);
        if (!match.Success)
            return null;

        try
        {
            return FrontMatterDeserializer.Deserialize<PostFrontMatter>(match.Groups[1].Value.Trim());
        }
        catch
        {
            return null;
        }
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
        var htmlBody = match.Groups[2].Value.Trim().ToHtml();

        try
        {
            var header = FrontMatterDeserializer.Deserialize<T>(yamlContent);
            return (header, htmlBody);
        }
        catch
        {
            return (null, htmlBody);
        }
    }

    public static Post ToPost(this string markdown, string normalizedPath, PathSettings pathSettings, PostFrontMatter? englishFrontMatter = null, string? languageOverride = null)
    {
        var (frontMatter, htmlContent) = markdown.ToHtmlWithPostFrontMatter();
        htmlContent = htmlContent.ResolveHtmlAssetUrls();

        var fileName = Path.GetFileNameWithoutExtension(normalizedPath);
        var (_, languageFromFile) = fileName.ParseLanguageFromFileName();
        var language = languageOverride ?? languageFromFile;
        var mergedFrontMatter = PostFrontMatter.MergeForLanguage(englishFrontMatter, frontMatter, language);
        var post = Post.Create(normalizedPath, htmlContent, mergedFrontMatter, language);
        post.Cover = ContentPathExtensions.ResolveAssetUrl(post.Cover);
        post.Thumbnail = ContentPathExtensions.ResolveAssetUrl(post.Thumbnail);
        return post;
    }

    public static AboutMe ToAboutMe(this string markdown, PathSettings pathSettings, string markdownRelativePath = "AboutMe.md")
    {
        var (frontMatter, htmlContent) = markdown.ToHtmlWithFrontMatter<AboutMeFrontMatter>();
        htmlContent = htmlContent.ResolveHtmlAssetUrls();

        var about = AboutMe.Create(frontMatter, htmlContent);
        about.Avatar = ContentPathExtensions.ResolveAssetUrl(about.Avatar);
        return about;
    }

    public static string ToHtmlBody(this string markdown, PathSettings pathSettings) =>
        markdown.WithoutFrontMatter().ToHtml().ResolveHtmlAssetUrls();

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
