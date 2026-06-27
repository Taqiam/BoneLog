namespace BoneLog.Tools;

public static class PathExtensions
{
    public const string CategorySeparator = " / ";
    public const string DefaultLanguage = "en";

    public static string NormalizeRelativePath(this string path) => path.TrimStart('/').Replace('\\', '/');

    public static string ToMarkdownFetchPath(this string basePath, string relativePath)
    {
        var normalized = relativePath.NormalizeRelativePath();
        var root = basePath.TrimEnd('/');
        return $"{root}/{normalized}.md";
    }

    public static (string BaseName, string Language) ParseLanguageFromFileName(this string fileNameWithoutExtension)
    {
        if (string.IsNullOrWhiteSpace(fileNameWithoutExtension))
            return (fileNameWithoutExtension, DefaultLanguage);

        var lastDot = fileNameWithoutExtension.LastIndexOf('.');
        if (lastDot > 0)
        {
            var suffix = fileNameWithoutExtension[(lastDot + 1)..];
            if (suffix.Length == 2 && suffix.All(char.IsLetter))
                return (fileNameWithoutExtension[..lastDot], suffix.ToLowerInvariant());
        }

        return (fileNameWithoutExtension, DefaultLanguage);
    }

    public static string SlugToTitle(this string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return slug;

        var words = slug.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (words.Length == 0) return slug;

        return string.Join(' ', words.Select(static w => char.ToUpperInvariant(w[0]) + (w.Length > 1 ? w[1..].ToLowerInvariant() : "")));
    }

    public static string ApplyIgnoreCache(this string url, bool ignoreCache)
    {
        if (ignoreCache)
            url += (url.Contains('?') ? "&" : "?") + $"nocache={DateTime.UtcNow.Ticks}";
        return url;
    }

    public static bool IsAbsoluteWebUrl(this string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || url.StartsWith('#'))
            return false;

        if (url.StartsWith("//", StringComparison.Ordinal))
            return true;

        return url.Contains("://", StringComparison.Ordinal);
    }

    /// <summary>App routes and nav links: keep full URLs; otherwise strip leading slashes so &lt;base href&gt; applies.</summary>
    public static string ToAppRelativeUrl(this string? url)
    {
        if (string.IsNullOrWhiteSpace(url) || url.StartsWith('#') || url.IsAbsoluteWebUrl())
            return url ?? "";

        return url.TrimStart('/');
    }
}
