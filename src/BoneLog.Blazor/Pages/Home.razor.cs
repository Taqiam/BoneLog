using BoneLog.Abstractions;
using BoneLog.Blazor.Dtos;
using BoneLog.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace BoneLog.Blazor.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] IPostReader Reader { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] SiteConfig config { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "q")]
    public string? Q { get; set; }

    private bool isLoading = true;
    private string searchQuery = string.Empty;

    private SearchQueryParts parsedSearch = SearchQueryParts.Empty;

    private string? SelectedCategory => parsedSearch.Categories.LastOrDefault();

    private string? SelectedLanguage => parsedSearch.Languages.LastOrDefault();

    private PostIndex[] allPosts = [];
    public PostIndex[] Posts = [];

    private bool ShowLanguageSidebar =>
        config.FeaturesOrDefault.EnableMultilanguage
        && config.FeaturesOrDefault.LanguageSidebar
        && allPosts.Select(p => p.Language).Distinct(StringComparer.OrdinalIgnoreCase).Count() > 1;

    private bool ShowSidebar =>
        config.FeaturesOrDefault.CategorySidebar || ShowLanguageSidebar;


    #region Hooks

    protected override async Task OnInitializedAsync()
    {
        Nav.LocationChanged += OnLocationChanged;
        await ReloadPosts(false);
    }

    protected override void OnParametersSet()
    {
        searchQuery = Q ?? string.Empty;
        ApplySearchQuery();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) =>
        _ = InvokeAsync(() =>
        {
            ApplyQueryFromUri(e.Location);
            StateHasChanged();
            return Task.CompletedTask;
        });

    private void ApplyQueryFromUri(string uriString)
    {
        var uri = Nav.ToAbsoluteUri(uriString);
        searchQuery = QueryHelpers.ParseQuery(uri.Query).TryGetValue("q", out var searchValue)
            ? searchValue.ToString()
            : string.Empty;
        ApplySearchQuery();
    }

    public void Dispose() => Nav.LocationChanged -= OnLocationChanged;

    #endregion

    #region Events

    private void OnPageInputChanged(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out int val))
        {
            CurrentPage = val;
        }
    }

    private void OnSearchInput(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString()?.Trim() ?? "";
        ApplySearchQuery();
    }

    #endregion

    #region Load

    public async Task LoadAsync(bool ignoreCache = false)
    {
        isLoading = true;
        allPosts = await Reader.GetIndex(ignoreCache);
        Posts = allPosts;
        isLoading = false;
    }

    private async Task ReloadPosts(bool ignoreCache)
    {
        await LoadAsync(ignoreCache);
        ApplySearchQuery();
    }

    #endregion

    #region Search

    private const string TagSearchPrefix = "Tag:";
    private const string CatSearchPrefix = "Cat:";
    private const string LangSearchPrefix = "Lang:";

    private void ApplySearchQuery()
    {
        parsedSearch = SearchQueryParts.Parse(searchQuery, config.FeaturesOrDefault.EnableMultilanguage);

        if (parsedSearch.IsEmpty)
        {
            Posts = allPosts;
            CurrentPage = 1;
            return;
        }

        IEnumerable<PostIndex> results = allPosts;

        foreach (var language in parsedSearch.Languages)
        {
            results = results.Where(p =>
                p.Language.Equals(language, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var category in parsedSearch.Categories)
        {
            results = results.Where(p =>
                p.Category?.Contains(category, StringComparison.OrdinalIgnoreCase) == true);
        }

        foreach (var tag in parsedSearch.Tags)
        {
            results = results.Where(p =>
                p.Tags?.Any(t => t.Contains(tag, StringComparison.OrdinalIgnoreCase)) == true);
        }

        foreach (var term in parsedSearch.FreeText)
        {
            results = results.Where(p => MatchesFreeText(p, term));
        }

        Posts = results
            .OrderByDescending(p => p.Date)
            .ToArray();

        CurrentPage = 1;
    }

    private static bool MatchesFreeText(PostIndex post, string term) =>
        post.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
        || post.ShortDescription?.Contains(term, StringComparison.OrdinalIgnoreCase) == true
        || (post.Tags?.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)) ?? false);

    private sealed record SearchQueryParts(
        IReadOnlyList<string> Tags,
        IReadOnlyList<string> Categories,
        IReadOnlyList<string> Languages,
        IReadOnlyList<string> FreeText)
    {
        public static readonly SearchQueryParts Empty = new([], [], [], []);

        public bool IsEmpty =>
            Tags.Count == 0
            && Categories.Count == 0
            && Languages.Count == 0
            && FreeText.Count == 0;

        public static SearchQueryParts Parse(string query, bool enableLanguage)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Empty;

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

        private static bool TryReadFilter(
            string query,
            ref int pos,
            string prefix,
            List<string> values)
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
            query.AsSpan(index).StartsWith(TagSearchPrefix, StringComparison.OrdinalIgnoreCase)
            || query.AsSpan(index).StartsWith(CatSearchPrefix, StringComparison.OrdinalIgnoreCase)
            || query.AsSpan(index).StartsWith(LangSearchPrefix, StringComparison.OrdinalIgnoreCase);
    }


    private void ClearSearch() => Nav.NavigateTo("/", replace: true);
    #endregion

    #region Paging

    const int ITEMS_PER_PAGE = 10;

    public int CurrentPage { get; set { field = Math.Clamp(value, 1, Math.Max(PageCount, 1)); } } = 1;
    public int PageCount => (int)Math.Ceiling((double)Posts.Length / ITEMS_PER_PAGE);
    public PostIndex[] GetCurrentPage() => Posts.Skip((CurrentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).ToArray();

    #endregion
}
