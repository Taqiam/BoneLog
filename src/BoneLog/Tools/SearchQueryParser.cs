namespace BoneLog.Tools;

public sealed record SearchQueryParser(IReadOnlyList<string> Tags, IReadOnlyList<string> Categories, IReadOnlyList<string> Languages, IReadOnlyList<string> FreeText)
{
    public const string TagSearchPrefix = "Tag:";
    public const string CatSearchPrefix = "Cat:";
    public const string LangSearchPrefix = "Lang:";

    public static readonly SearchQueryParser Empty = new([], [], [], []);

    public bool IsEmpty => Tags.Count == 0 && Categories.Count == 0 && Languages.Count == 0 && FreeText.Count == 0;

    public static SearchQueryParser Parse(string query, bool enableLanguage)
    {
        if (string.IsNullOrWhiteSpace(query)) return Empty;

        var tags = new List<string>();
        var categories = new List<string>();
        var languages = new List<string>();
        var freeText = new List<string>();

        var pos = 0;
        while (pos < query.Length)
        {
            if (char.IsWhiteSpace(query[pos]))
            {
                pos++;
                continue;
            }

            if (enableLanguage && TryReadFilter(query, ref pos, LangSearchPrefix, languages))
                continue;

            if (TryReadFilter(query, ref pos, CatSearchPrefix, categories))
                continue;

            if (TryReadFilter(query, ref pos, TagSearchPrefix, tags))
                continue;

            var end = FindNextFilterIndex(query, pos);
            var chunk = query[pos..end].Trim();
            if (chunk.Length > 0)
            {
                freeText.AddRange(chunk.Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }

            pos = end;
        }

        return new(tags, categories, languages, freeText);
    }

    private static bool TryReadFilter(string query, ref int pos, string prefix, List<string> values)
    {
        if (!query.AsSpan(pos).StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return false;

        pos += prefix.Length;
        var end = FindNextFilterIndex(query, pos);
        var value = query[pos..end].Trim();
        if (value.Length > 0)
            values.Add(value);

        pos = end;
        return true;
    }

    private static int FindNextFilterIndex(string query, int start)
    {
        for (var i = start; i < query.Length; i++)
        {
            if (!char.IsWhiteSpace(query[i]))
                continue;

            var j = i + 1;
            while (j < query.Length && char.IsWhiteSpace(query[j]))
                j++;

            if (j >= query.Length)
                return query.Length;

            if (StartsWithFilterPrefix(query, j))
                return i;
        }

        return query.Length;
    }

    private static bool StartsWithFilterPrefix(string query, int index) =>
        query.AsSpan(index).StartsWith(TagSearchPrefix, StringComparison.OrdinalIgnoreCase) ||
        query.AsSpan(index).StartsWith(CatSearchPrefix, StringComparison.OrdinalIgnoreCase) ||
        query.AsSpan(index).StartsWith(LangSearchPrefix, StringComparison.OrdinalIgnoreCase);
}
