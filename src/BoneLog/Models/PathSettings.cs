namespace BoneLog.Models;

public record PathSettings(
    string BaseDataPath,
    string PostsPath,
    string IndexPath,
    string AboutMePath)
{
    public string GetPostsPath() => ResolvePath(PostsPath);
    public string GetIndexPath() => ResolvePath(IndexPath);
    public string GetAboutMePath() => ResolvePath(AboutMePath);

    private string ResolvePath(string path)
    {
        if (IsAbsoluteUrl(path)) return path;

        var basePath = BaseDataPath.TrimEnd('/');
        var relative = path.TrimStart('/');
        return $"{basePath}/{relative}";
    }

    private static bool IsAbsoluteUrl(string path) =>
        Uri.TryCreate(path, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}
