namespace BoneLog.Tools;

public static class PostIdExtensions
{
    public static bool IsPostId(this string? value) => TryNormalizePostId(value, out _);

    public static bool TryNormalizePostId(this string? value, out string normalized)
    {
        normalized = "";

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var trimmed = value.Trim();
        if (trimmed.Length == 0 || !trimmed.All(static c => c is >= '0' and <= '9'))
            return false;

        normalized = trimmed;
        return true;
    }

    public static string? NormalizePostIdOrNull(this string? value) => value.TryNormalizePostId(out var normalized) ? normalized : null;

    public static bool PostIdEquals(this string? left, string? right) =>
        left.TryNormalizePostId(out var leftNormalized)
        && right.TryNormalizePostId(out var rightNormalized)
        && leftNormalized == rightNormalized;

    public static int ComparePostIds(string? left, string? right)
    {
        var leftValid = left.TryNormalizePostId(out var leftNormalized);
        var rightValid = right.TryNormalizePostId(out var rightNormalized);

        if (leftValid && rightValid)
        {
            if (ulong.TryParse(leftNormalized, System.Globalization.NumberStyles.None, null, out var leftValue)
                && ulong.TryParse(rightNormalized, System.Globalization.NumberStyles.None, null, out var rightValue))
            {
                var compare = leftValue.CompareTo(rightValue);
                if (compare != 0)
                    return compare;
            }

            return string.Compare(leftNormalized, rightNormalized, StringComparison.Ordinal);
        }

        if (leftValid != rightValid)
            return leftValid ? -1 : 1;

        return string.Compare(left, right, StringComparison.Ordinal);
    }
}
