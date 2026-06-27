using BoneLog.Models;
using System.Text.RegularExpressions;

namespace BoneLog.Tools;

public static partial class ContentPathExtensions
{
    [GeneratedRegex("""(<img\b[^>]*?src=")([^"]+)("[^>]*>)""", RegexOptions.IgnoreCase)]
    private static partial Regex ImgSrcRegex();

    public static string NormalizeBaseDir(string? baseDir)
    {
        var dir = string.IsNullOrWhiteSpace(baseDir) ? "/" : baseDir.Trim();
        if (!dir.StartsWith('/'))
            dir = '/' + dir;
        if (!dir.EndsWith('/'))
            dir += '/';
        return dir;
    }

    /// <summary>
    /// Site asset paths: full URL unchanged; root paths like /images/x.png become images/x.png (relative to &lt;base href&gt;).
    /// </summary>
    public static string? ResolveAssetUrl(string? assetPath)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
            return assetPath;

        if (assetPath.StartsWith("//", StringComparison.Ordinal)
            || assetPath.IsAbsoluteWebUrl()
            || assetPath.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            return assetPath;
        }

        return assetPath.TrimStart('/');
    }

    public static string ResolveHtmlAssetUrls(this string html)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        return ImgSrcRegex().Replace(html, match =>
        {
            var resolved = ResolveAssetUrl(match.Groups[2].Value) ?? match.Groups[2].Value;
            return $"{match.Groups[1].Value}{resolved}{match.Groups[3].Value}";
        });
    }

    public static string? ResolveThumbnail(this PostIndex post) =>
        ResolveAssetUrl(post.Thumbnail);
}
