namespace BoneLog.Tools;

public static class PathExtensions
{
    public const string CategorySeparator = " / ";

    public static string NormalizeRelativePath(this string path) => path.TrimStart('/').Replace('\\', '/');

    public static string ToMarkdownFetchPath(this string basePath, string relativePath)
    {
        var normalized = relativePath.NormalizeRelativePath();
        var root = basePath.TrimEnd('/');
        return $"{root}/{normalized}.md";
    }

    public static string? CategoryFromPath(this string path)
    {
        var parts = path.NormalizeRelativePath().Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1) return null;

        var folderSegments = parts[..^1];
        if (folderSegments.Length == 0) return null;

        return string.Join(CategorySeparator, folderSegments.Select(s => s.SlugToTitle()));
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
}