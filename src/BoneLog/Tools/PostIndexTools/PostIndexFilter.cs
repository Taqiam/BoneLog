using BoneLog.Models;

namespace BoneLog.Tools;

public static class PostIndexFilter
{
    public static PostIndex[] Apply(this IReadOnlyList<PostIndex> allPosts, SearchQueryParser parsed)
    {
        if (parsed.IsEmpty) return [.. allPosts];

        IEnumerable<PostIndex> results = allPosts;

        foreach (var term in parsed.FreeText)
            results = results.Where(p => MatchesFreeText(p, term));

        foreach (var tag in parsed.Tags)
            results = results.Where(p => p.Tags?.Any(t => t.Contains(tag, StringComparison.OrdinalIgnoreCase)) == true);

        foreach (var category in parsed.Categories)
            results = results.Where(p => p.Category?.Contains(category, StringComparison.OrdinalIgnoreCase) == true);

        foreach (var language in parsed.Languages)
            results = results.Where(p => p.Language.Equals(language, StringComparison.OrdinalIgnoreCase));

        return results.OrderByDescending(p => p.Date).ToArray();

    }
    private static bool MatchesFreeText(PostIndex post, string term) =>
         post.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
         post.ShortDescription?.Contains(term, StringComparison.OrdinalIgnoreCase) == true ||
         (post.Tags?.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)) ?? false);

}
