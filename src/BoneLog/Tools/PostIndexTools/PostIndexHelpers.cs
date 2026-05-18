using BoneLog.Models;

namespace BoneLog.Tools;

public static class PostIndexHelpers
{
    public static bool HaveTags(this IEnumerable<PostIndex> posts) => posts.Any(p => p.Tags is { Length: > 0 });
    public static bool HaveLanguages(this IEnumerable<PostIndex> posts) => posts.Select(p => p.Language).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;
    public static bool HaveCategories(this IEnumerable<PostIndex> posts) => posts.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;

}
