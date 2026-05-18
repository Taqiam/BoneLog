using BoneLog.Models;
using Markdig;
using System.Text.RegularExpressions;

namespace BoneLog.Tools;

public static partial class MarkdownExtensions
{
    [GeneratedRegex(@"^---\s*\n(.*?)\n---\s*\n(.*)$", RegexOptions.Singleline)]
    private static partial Regex FrontMatterRegex();

    public static string ToHtml(this string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdown, pipeline).ApplyAutoDirection();
    }

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

    public static Post ToPost(this string markdown, string normalizedPath)
    {
        var (frontMatter, htmlContent) = markdown.ToHtmlWithPostFrontMatter();
        return Post.Create(normalizedPath, htmlContent, frontMatter);
    }

    public static AboutMe ToAboutMe(this string markdown)
    {
        var (frontMatter, htmlContent) = markdown.ToHtmlWithFrontMatter<AboutMeFrontMatter>();
        return AboutMe.Create(frontMatter, htmlContent);
    }

    public static string ToHtmlBody(this string markdown) =>
        markdown.WithoutFrontMatter().ToHtml();

    private static string ApplyAutoDirection(this string html)
    {
        const string tags = "p|div|span|h1|h2|h3|h4|h5|h6";
        var regex = new Regex($@"<({tags})(?![^>]*dir=)([^>]*)>(\s*[\u0600-\u06FF])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return regex.Replace(html, @"<$1 dir=""rtl""$2>$3");
    }
}
