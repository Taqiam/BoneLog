using BoneLog.Abstractions;
using BoneLog.Blazor.Dtos;
using BoneLog.Models;
using BoneLog.Tools;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;

namespace BoneLog.Blazor.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] IBlogContentProvider Reader { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] SiteConfig Config { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "q")]
    public string? Q { get; set; }

    [SupplyParameterFromQuery(Name = "lang")]
    public string? Lang { get; set; }

    private bool isLoading = true;
    private string searchQuery = string.Empty;

    private PostIndex[] allPosts = [];
    private PostIndex[] listingPosts = [];
    public PostIndex[] Posts = [];

    private SearchQueryParser parsedSearch = SearchQueryParser.Empty;

    private string? SelectedCategory => parsedSearch.Categories.LastOrDefault();
    private string? SelectedTag => parsedSearch.Tags.LastOrDefault();
    private string? EffectiveLanguageFilter => PostIndexHelpers.ResolveLanguageFilter(Lang, parsedSearch);
    private string? SelectedLanguage => EffectiveLanguageFilter;

    private bool ShowLanguageSidebar => allPosts.HaveLanguages();
    private bool ShowTagSidebar => Config.FeaturesOrDefault.TagSidebar && listingPosts.HaveTags();
    private bool ShowCategories => Config.FeaturesOrDefault.CategorySidebar && listingPosts.HaveCategories();
    private bool ShowSidebar => ShowLanguageSidebar || ShowCategories || ShowTagSidebar;


    #region Lifecycle Hooks

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

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) => _ = InvokeAsync(() =>
        {
            ApplyQueryFromUri(e.Location);
            StateHasChanged();
            return Task.CompletedTask;
        });
    

    private void ApplyQueryFromUri(string uriString)
    {
        var uri = Nav.ToAbsoluteUri(uriString);
        var query = QueryHelpers.ParseQuery(uri.Query);
        searchQuery = query.TryGetValue("q", out var searchValue)
            ? searchValue.ToString()
            : string.Empty;
        Lang = query.TryGetValue("lang", out var langValue)
            ? langValue.ToString()
            : null;
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
        Nav.NavigateTo(BuildSearchUri(searchQuery));
    }

    #endregion

    #region Load Posts

    public async Task LoadAsync(bool ignoreCache = false)
    {
        isLoading = true;
        allPosts = await Reader.GetIndex(ignoreCache);
        ApplySearchQuery();
        isLoading = false;
    }

    private async Task ReloadPosts(bool ignoreCache)
    {
        await LoadAsync(ignoreCache);
    }

    #endregion

    #region Search

    private void ApplySearchQuery()
    {
        parsedSearch = SearchQueryParser.Parse(searchQuery);
        var languageFilter = EffectiveLanguageFilter;
        listingPosts = allPosts.FilterByLanguage(languageFilter);
        Posts = listingPosts.ApplySearch(parsedSearch, languageFilter);
        CurrentPage = 1;
    }

    private void ClearSearch() => Nav.NavigateTo(BuildSearchUri(string.Empty), replace: true);

    private string BuildSearchUri(string query)
    {
        var uri = Nav.ToAbsoluteUri(Nav.Uri);
        var parameters = QueryHelpers.ParseQuery(uri.Query);

        if (string.IsNullOrWhiteSpace(query))
            parameters.Remove("q");
        else
            parameters["q"] = query;

        if (PostIndexHelpers.IsAllLanguagesFilter(Lang))
            parameters.Remove("lang");
        else if (!string.IsNullOrWhiteSpace(Lang))
            parameters["lang"] = Lang;

        return QueryHelpers.AddQueryString(uri.GetLeftPart(UriPartial.Path), parameters);
    }

    #endregion

    #region Paging

    public int CurrentPage
    {
get;
        set { field = PagingHelper.ClampPage(value, PageCount); }
    } = 1;

    public int PageCount => PagingHelper.GetPageCount(Posts.Length,Config.PostsPerPage);
    public PostIndex[] GetCurrentPage() => PagingHelper.GetPage(Posts, CurrentPage, Config.PostsPerPage);

    #endregion
}
