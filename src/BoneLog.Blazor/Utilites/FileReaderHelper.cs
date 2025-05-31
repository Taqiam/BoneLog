using Markdig;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BoneLog.Blazor.Utilites;

public static partial class FileReaderHelper
{
    [GeneratedRegex(@"^---\s*\n(.*?)\n---\s*\n(.*)$", RegexOptions.Singleline)]
    private static partial Regex front_matter_regex();

    public static string MarkdownToHtml(this string markdown)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        return Markdown.ToHtml(markdown,pipeline).ApplyAutoDirection();
    }
    
    public static string RemoveYamlHeader(this string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return markdown;

        var match = front_matter_regex().Match(markdown);
        return match.Success ? match.Groups[2].Value.TrimStart() : markdown;
    }

    private static string ApplyAutoDirection(this string html)
    {
        string tags = "p|div|span|h1|h2|h3|h4|h5|h6";
        var regex = new Regex($@"<({tags})(?![^>]*dir=)([^>]*)>(\s*[\u0600-\u06FF])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        return regex.Replace(html, @"<$1 dir=""rtl""$2>$3");
    }

    public static (T?, string) ParseMarkdownToHtmlWithHeader<T>(this string markdown) where T : class
    {
        if(string.IsNullOrWhiteSpace(markdown))
            return (null, markdown);

        var match = front_matter_regex().Match(markdown);
        if(!match.Success)
        {
            string htmlContent = markdown.MarkdownToHtml();
            return (null, htmlContent);
        }

        var yamlContent = match.Groups[1].Value.Trim();
        var markdownBody = match.Groups[2].Value.Trim();

        var htmlBody = MarkdownToHtml(markdownBody);

        var deserializer = new DeserializerBuilder()
          .WithNamingConvention(CamelCaseNamingConvention.Instance)
          .IgnoreUnmatchedProperties()
          .Build();

        try
        {
            var header = deserializer.Deserialize<T>(yamlContent);
            return (header, htmlBody);
        }
        catch (Exception ex)
        {
            // in case of deserializer throws any exception we should return null.
            return (null, htmlBody);
        }
    }
}
