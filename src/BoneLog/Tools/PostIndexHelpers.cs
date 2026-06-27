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

    public static bool HaveCategories(this IEnumerable<PostIndex> posts) =>
        posts.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;

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

    public static bool HaveLanguages(this IEnumerable<PostIndex> posts) =>
        posts.SelectMany(p => p.Languages).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;

    public static PostIndexLanguageEntry[] GetLanguages(this IEnumerable<PostIndex> posts)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var post in posts)
        {
            foreach (var language in post.Languages)
            {
                counts.TryGetValue(language, out var count);
                counts[language] = count + 1;
            }
        }

        return counts
            .Select(kv => new PostIndexLanguageEntry(kv.Key, kv.Value))
            .OrderBy(e => e.Language, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static bool HasLanguage(this PostIndex post, string language) =>
        post.Languages.Any(l => l.Equals(language, StringComparison.OrdinalIgnoreCase));

    public static string GetMetadataFilePath(this PostIndex post)
    {
        if (post.FilePaths.TryGetValue(PathExtensions.DefaultLanguage, out var englishPath))
            return englishPath;

        return post.FilePaths.Values.First();
    }

    public static string GetDisplayTitle(this PostIndex post, string? languageFilter)
    {
        if (ShouldUseEnglishTitle(languageFilter))
            return post.Title;

        if (!string.IsNullOrWhiteSpace(languageFilter)
            && post.LocalizedTitles?.TryGetValue(languageFilter, out var localizedTitle) == true
            && !string.IsNullOrWhiteSpace(localizedTitle))
        {
            return localizedTitle;
        }

        return post.Title;
    }

    public static string GetLinkLanguage(this PostIndex post, string? languageFilter)
    {
        if (!string.IsNullOrWhiteSpace(languageFilter)
            && !IsAllLanguagesFilter(languageFilter)
            && post.HasLanguage(languageFilter))
        {
            return languageFilter;
        }

        if (post.HasLanguage(PathExtensions.DefaultLanguage))
            return PathExtensions.DefaultLanguage;

        return post.Languages.OrderBy(static l => l, StringComparer.OrdinalIgnoreCase).First();
    }

    public static PostIndex[] FilterByLanguage(this IEnumerable<PostIndex> posts, string? languageFilter)
    {
        if (IsAllLanguagesFilter(languageFilter) || IsEnglishFilter(languageFilter))
            return posts.ToArray();

        return posts
            .Where(p => p.HasLanguage(languageFilter!))
            .ToArray();
    }

    public static bool IsAllLanguagesFilter(string? languageFilter) =>
        string.IsNullOrWhiteSpace(languageFilter)
        || languageFilter.Equals("all", StringComparison.OrdinalIgnoreCase);

    public static bool IsEnglishFilter(string? languageFilter) =>
        languageFilter?.Equals(PathExtensions.DefaultLanguage, StringComparison.OrdinalIgnoreCase) == true;

    public static string[] GetNonEnglishLanguages(this PostIndex post) =>
        post.Languages
            .Where(l => !l.Equals(PathExtensions.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            .OrderBy(static l => l, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    public static string? ResolveLanguageFilter(string? langQuery, SearchQueryParser parsedSearch)
    {
        var fromSearch = parsedSearch.Languages.LastOrDefault();
        if (!string.IsNullOrWhiteSpace(fromSearch))
            return fromSearch;

        return IsAllLanguagesFilter(langQuery) ? null : langQuery;
    }

    private static bool ShouldUseEnglishTitle(string? languageFilter) =>
        IsAllLanguagesFilter(languageFilter) || IsEnglishFilter(languageFilter);

    #endregion

    #region Search

    public static PostIndex[] ApplySearch(this IReadOnlyList<PostIndex> allPosts, SearchQueryParser parsed, string? languageFilter = null)
    {
        IEnumerable<PostIndex> filtered = parsed.IsEmpty
            ? allPosts
            : allPosts.Where(p => Matches(p, parsed));

        var ordered = parsed.IsEmpty
            ? filtered
            : filtered.OrderByDescending(p => p.Date);

        return ordered
            .Select(p => p.WithDisplayTitle(languageFilter))
            .ToArray();
    }

    private static PostIndex WithDisplayTitle(this PostIndex post, string? languageFilter)
    {
        var title = post.GetDisplayTitle(languageFilter);
        if (title.Equals(post.Title, StringComparison.Ordinal))
            return post;

        return new PostIndex
        {
            Id = post.Id,
            Slug = post.Slug,
            Title = title,
            ShortDescription = post.ShortDescription,
            Category = post.Category,
            Tags = post.Tags,
            Date = post.Date,
            Thumbnail = post.Thumbnail,
            Languages = post.Languages,
            LocalizedTitles = post.LocalizedTitles,
            FilePaths = post.FilePaths
        };
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
            if (!post.HasLanguage(language))
                return false;
        }

        return true;
    }

    private static bool MatchesFreeText(PostIndex post, string term) =>
        post.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
        || (post.ShortDescription?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)
        || (post.LocalizedTitles?.Values.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)) == true);

    #endregion

    public static string ToPostUrl(this PostIndex post, string language) =>
        $"posts/{language}/{post.Id.NormalizePostIdOrNull() ?? post.Id}/{post.Slug}";

    public static string ToPostUrl(this Post post) =>
        $"posts/{post.Language}/{post.Id.NormalizePostIdOrNull() ?? post.Id}/{post.Slug}";

    public static string ToPostUrl(string id, string slug, string language) =>
        $"posts/{language}/{id.NormalizePostIdOrNull() ?? id}/{slug}";

    public static Dictionary<string, string> BuildPostUrlMap(this IEnumerable<PostIndex> posts)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var post in posts)
        {
            map[post.Slug] = post.ToPostUrl(post.GetLinkLanguage(null));

            foreach (var (language, filePath) in post.FilePaths)
            {
                var url = post.ToPostUrl(language);
                map[filePath] = url;

                var normalizedPath = filePath.NormalizeRelativePath();
                var fileName = Path.GetFileNameWithoutExtension(normalizedPath);
                var (baseName, _) = fileName.ParseLanguageFromFileName();
                map[baseName] = post.ToPostUrl(post.GetLinkLanguage(language));

                var directory = Path.GetDirectoryName(normalizedPath)?.Replace('\\', '/');
                if (!string.IsNullOrEmpty(directory))
                    map[$"{directory}/{baseName}"] = post.ToPostUrl(post.GetLinkLanguage(language));
            }
        }

        return map;
    }
}

public readonly record struct PostIndexLanguageEntry(string Language, int Count);
public readonly record struct PostIndexTagEntry(string Tag, int Count);
