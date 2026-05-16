namespace BoneLog.Tools;

public static class PostPathHelper
{
    public const string CategorySeparator = " / ";

    public static string? CategoryFromPath(string path)
    {
        var parts = path.Replace('\\', '/').Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 1)
            return null;

        var folderSegments = parts[..^1];
        if (folderSegments.Length == 0)
            return null;

        return string.Join(CategorySeparator, folderSegments.Select(FolderNameToTitle));
    }

    public static string FolderNameToTitle(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName)) return folderName;

        var words = folderName.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (words.Length == 0) return folderName;

        return string.Join(' ', words.Select(static w => char.ToUpperInvariant(w[0]) + (w.Length > 1 ? w[1..].ToLowerInvariant() : "")));
    }
}
