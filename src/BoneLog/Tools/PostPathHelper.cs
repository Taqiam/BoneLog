namespace BoneLog.Tools;

public static class PostPathHelper
{
    public static string? CategoryFromPath(string path)
    {
        var segment = path.Replace('\\', '/').Trim('/').Split('/').FirstOrDefault();
        return string.IsNullOrWhiteSpace(segment) ? null : FolderNameToTitle(segment);
    }

    public static string FolderNameToTitle(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName)) return folderName;

        var words = folderName.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (words.Length == 0) return folderName;

        return string.Join(' ', words.Select(static w => char.ToUpperInvariant(w[0]) + (w.Length > 1 ? w[1..].ToLowerInvariant() : "")));
    }
}
