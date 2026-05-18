using BoneLog.Models;

namespace BoneLog.Tools;

public static class PostIndexHelpers
{

    #region Tags

    public static bool HaveTags(this IEnumerable<PostIndex> posts) => posts.Any(p => p.Tags is { Length: > 0 });

    public static PostIndexTagEntry[] GetTags(this IEnumerable<PostIndex> posts)
    {
        var tagCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var post in posts)
        {
            if (post.Tags is not { Length: > 0 })
                continue;

            foreach (var tag in post.Tags.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                tagCounts.TryGetValue(tag, out var count);
                tagCounts[tag] = count + 1;
            }
        }

        return tagCounts
            .Select(kv => new PostIndexTagEntry(kv.Key, kv.Value))
            .OrderBy(e => e.Tag, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    #endregion

    #region Categories
    public static bool HaveCategories(this IEnumerable<PostIndex> posts) => posts.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;

    public static Category[] GetCategories(this IEnumerable<PostIndex> posts)
    {
        var roots = new Dictionary<string, CategoryNode>(StringComparer.OrdinalIgnoreCase);

        foreach (var post in posts)
        {
            if (string.IsNullOrWhiteSpace(post.Category))
                continue;

            var segments = post.Category.Split(PathExtensions.CategorySeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (segments.Length == 0)
                continue;

            var level = roots;
            CategoryNode? node = null;

            foreach (var segment in segments)
            {
                if (!level.TryGetValue(segment, out node))
                {
                    node = new CategoryNode(segment);
                    level[segment] = node;
                }

                level = node.Children;
            }

            node!.DirectPosts++;
        }

        return roots.Values.OrderBy(n => n.Title, StringComparer.OrdinalIgnoreCase).Select(ToCategory).ToArray();
    }

    private static Category ToCategory(CategoryNode node)
    {
        var children = node.Children.Values.OrderBy(c => c.Title, StringComparer.OrdinalIgnoreCase).Select(ToCategory).ToArray();
        var childTotal = children.Sum(c => c.NumberOfPosts);

        return new()
        {
            Title = node.Title,
            NumberOfPosts = node.DirectPosts + childTotal,
            SubCategories = children.Length > 0 ? children : null
        };
    }

    private sealed class CategoryNode(string title)
    {
        public string Title { get; } = title;
        public int DirectPosts { get; set; }
        public Dictionary<string, CategoryNode> Children { get; } = new(StringComparer.OrdinalIgnoreCase);
    }


    #endregion

    #region Languages

    public static bool HaveLanguages(this IEnumerable<PostIndex> posts) => posts.Select(p => p.Language).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;

    public static PostIndexLanguageEntry[] GetLanguages(this IEnumerable<PostIndex> posts) =>
        posts
            .GroupBy(p => p.Language, StringComparer.OrdinalIgnoreCase)
            .Select(g => new PostIndexLanguageEntry(g.Key, g.Count()))
            .OrderBy(e => e.Language, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    #endregion

    #region Search

    public static PostIndex[] ApplySearch(this IReadOnlyList<PostIndex> allPosts, SearchQueryParser parsed)
    {
        if (parsed.IsEmpty)
            return [.. allPosts];

        return allPosts
            .Where(p => Matches(p, parsed))
            .OrderByDescending(p => p.Date)
            .ToArray();
    }

    private static bool Matches(PostIndex post, SearchQueryParser parsed)
    {
        foreach (var term in parsed.FreeText)
        {
            if (!MatchesFreeText(post, term))
                return false;
        }

        foreach (var tag in parsed.Tags)
        {
            if (post.Tags?.Any(t => t.Contains(tag, StringComparison.OrdinalIgnoreCase)) != true)
                return false;
        }

        foreach (var category in parsed.Categories)
        {
            if (post.Category?.Contains(category, StringComparison.OrdinalIgnoreCase) != true)
                return false;
        }

        foreach (var language in parsed.Languages)
        {
            if (!post.Language.Equals(language, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private static bool MatchesFreeText(PostIndex post, string term) =>
        post.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
        post.ShortDescription?.Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
        (post.Tags?.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)) ?? false);

    #endregion
}

public readonly record struct PostIndexLanguageEntry(string Language, int Count);
public readonly record struct PostIndexTagEntry(string Tag, int Count);
