using BoneLog.Models;
using System.Text.RegularExpressions;

namespace BoneLog.Tools;

public static partial class ContentPathExtensions
{
    [GeneratedRegex("""<img\b([^>]*?)\ssrc="([^"]+)"([^>]*)>""", RegexOptions.IgnoreCase)]
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

    public static bool ShouldKeepAssetPathAsIs(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return true;

        if (path.StartsWith("//", StringComparison.Ordinal))
            return true;

        if (Uri.TryCreate(path, UriKind.Absolute, out var uri))
        {
            return uri.Scheme == Uri.UriSchemeHttp
                || uri.Scheme == Uri.UriSchemeHttps
                || uri.Scheme == "data"
                || uri.Scheme == Uri.UriSchemeMailto;
        }

        return path.StartsWith('/');
    }

    public static string CombineRelative(string baseDirectory, string relativePath)
    {
        var baseParts = baseDirectory
            .Replace('\\', '/')
            .Trim('/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        var stack = new List<string>(baseParts);

        foreach (var part in relativePath.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            if (part == "..")
            {
                if (stack.Count > 0)
                    stack.RemoveAt(stack.Count - 1);
            }
            else if (part != ".")
            {
                stack.Add(part);
            }
        }

        return string.Join('/', stack);
    }

    public static string GetPostMarkdownDirectory(string postRelativePath, PathSettings settings)
    {
        var postsPrefix = settings.PostsPath.Trim().TrimEnd('/').TrimStart('/');
        var normalized = postRelativePath.NormalizeRelativePath().Trim('/');
        var postFolder = GetDirectoryPath(normalized);

        return string.IsNullOrEmpty(postFolder)
            ? postsPrefix
            : $"{postsPrefix}/{postFolder}";
    }

    public static string ResolvePostAssetPath(string postRelativePath, string assetPath, PathSettings settings)
    {
        if (string.IsNullOrWhiteSpace(assetPath) || ShouldKeepAssetPathAsIs(assetPath))
            return assetPath;

        var postDir = GetPostMarkdownDirectory(postRelativePath, settings);
        return CombineRelative(postDir, assetPath);
    }

    public static string ToDataAssetUrl(string pathFromDataRoot, PathSettings settings)
    {
        if (string.IsNullOrWhiteSpace(pathFromDataRoot) || ShouldKeepAssetPathAsIs(pathFromDataRoot))
            return pathFromDataRoot;

        var basePath = settings.BaseDataPath.TrimEnd('/');
        var relative = pathFromDataRoot.TrimStart('/');
        return $"{basePath}/{relative}";
    }

    public static string ToContentAssetUrl(string pathFromDataRoot, PathSettings settings)
    {
        if (string.IsNullOrWhiteSpace(pathFromDataRoot) || ShouldKeepAssetPathAsIs(pathFromDataRoot))
            return pathFromDataRoot;

        return IsSiteImagesPath(pathFromDataRoot, settings)
            ? ToSiteAssetUrl(pathFromDataRoot, settings)
            : ToDataAssetUrl(pathFromDataRoot, settings);
    }

    public static string? ResolvePostAssetUrl(string postRelativePath, string? assetPath, PathSettings settings)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
            return assetPath;

        if (ShouldKeepAssetPathAsIs(assetPath))
            return assetPath;

        var dataRelative = ResolvePostAssetPath(postRelativePath, assetPath, settings);
        return ToContentAssetUrl(dataRelative, settings);
    }

    public static string? ResolveDataFileAssetUrl(string markdownRelativePath, string? assetPath, PathSettings settings)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
            return assetPath;

        if (ShouldKeepAssetPathAsIs(assetPath))
            return assetPath;

        var fileDir = GetDirectoryPath(markdownRelativePath.NormalizeRelativePath());
        var dataRelative = string.IsNullOrEmpty(fileDir)
            ? CombineRelative("", assetPath)
            : CombineRelative(fileDir, assetPath);

        return ToContentAssetUrl(dataRelative, settings);
    }

    public static string ResolveHtmlAssetUrls(this string html, string contentRelativePath, PathSettings settings, bool isPost = true)
    {
        if (string.IsNullOrEmpty(html))
            return html;

        return ImgSrcRegex().Replace(html, match =>
        {
            var src = match.Groups[2].Value;
            var resolved = isPost
                ? ResolvePostAssetUrl(contentRelativePath, src, settings) ?? src
                : ResolveDataFileAssetUrl(contentRelativePath, src, settings) ?? src;

            return $"""<img{match.Groups[1].Value}src="{resolved}"{match.Groups[3].Value}>""";
        });
    }

    public static string? ResolveThumbnail(this PostIndex post, PathSettings settings) =>
        ResolvePostAssetUrl(post.Path, post.Thumbnail, settings);

    private static bool IsSiteImagesPath(string path, PathSettings settings)
    {
        var normalized = path.Replace('\\', '/').TrimStart('/');
        var postsPrefix = settings.PostsPath.Trim().Trim('/').TrimStart('/');

        var underPosts = normalized.StartsWith($"{postsPrefix}/", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals(postsPrefix, StringComparison.OrdinalIgnoreCase);

        return !underPosts && normalized.StartsWith("images/", StringComparison.OrdinalIgnoreCase);
    }

    private static string ToSiteAssetUrl(string relativePath, PathSettings settings)
    {
        var siteBase = GetSiteBaseFromDataPath(settings.BaseDataPath.TrimEnd('/'));
        return $"{siteBase}/{relativePath.TrimStart('/')}";
    }

    private static string GetSiteBaseFromDataPath(string dataBase)
    {
        if (Uri.TryCreate(dataBase, UriKind.Absolute, out var uri))
        {
            var segments = uri.AbsolutePath.TrimEnd('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0 && segments[^1].Equals("data", StringComparison.OrdinalIgnoreCase))
            {
                var parentPath = segments.Length == 1
                    ? ""
                    : "/" + string.Join('/', segments[..^1]);
                return $"{uri.Scheme}://{uri.Authority}{parentPath}";
            }

            return dataBase;
        }

        if (dataBase.EndsWith("/data", StringComparison.OrdinalIgnoreCase)
            || dataBase.EndsWith("\\data", StringComparison.OrdinalIgnoreCase))
        {
            return dataBase[..^5].TrimEnd('/');
        }

        return dataBase;
    }

    private static string GetDirectoryPath(string normalizedPath)
    {
        var directory = Path.GetDirectoryName(normalizedPath.Replace('/', Path.DirectorySeparatorChar));
        return string.IsNullOrEmpty(directory)
            ? ""
            : directory.Replace(Path.DirectorySeparatorChar, '/');
    }
}
