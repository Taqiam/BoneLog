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

    private bool isLoading = true;
    private string searchQuery = string.Empty;

    private PostIndex[] allPosts = [];
    public PostIndex[] Posts = [];

    private SearchQueryParser parsedSearch = SearchQueryParser.Empty;

    private string? SelectedCategory => parsedSearch.Categories.LastOrDefault();
    private string? SelectedLanguage => parsedSearch.Languages.LastOrDefault();
    private string? SelectedTag => parsedSearch.Tags.LastOrDefault();


    private bool ShowLanguageSidebar => Config.FeaturesOrDefault.EnableMultilanguage && Config.FeaturesOrDefault.LanguageSidebar && allPosts.HaveTags();
    private bool ShowTagSidebar => Config.FeaturesOrDefault.TagSidebar && allPosts.HaveLanguages();
    private bool ShowCategories => Config.FeaturesOrDefault.CategorySidebar && allPosts.HaveCategories();
    private bool ShowSidebar => ShowTagSidebar && ShowCategories && ShowLanguageSidebar;


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

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e) =>  _ = InvokeAsync(() =>
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

    #region Load Posts

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

    private void ApplySearchQuery()
    {
        parsedSearch = SearchQueryParser.Parse(searchQuery, Config.FeaturesOrDefault.EnableMultilanguage);
        Posts = PostIndexFilter.Apply(allPosts, parsedSearch);
        CurrentPage = 1;
    }

    private void ClearSearch() => Nav.NavigateTo("/", replace: true);

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