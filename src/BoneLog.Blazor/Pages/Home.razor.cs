using BoneLog.Blazor.Dtos;
using BoneLog.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BoneLog.Blazor.Pages;

public partial class Home : ComponentBase, IDisposable
{
    [Inject] HttpClient httpClient { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] SiteConfig config { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "q")]
    public string? Q { get; set; }

    private bool isLoading = true;
    private string searchQuery = string.Empty;

    private string? SelectedCategory =>
        searchQuery.StartsWith(CatSearchPrefix, StringComparison.OrdinalIgnoreCase)
            ? searchQuery[CatSearchPrefix.Length..].Trim()
            : null;

    private PostIndex[] allPosts = [];
    public PostIndex[] Posts = [];


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
        var path = config.IndexPath + (ignoreCache ? $"?nocache={Guid.NewGuid()}" : "");

        isLoading = true;
        var response = await httpClient.GetAsync(path);
        isLoading = false;
        if (!response.IsSuccessStatusCode) return;

        var json = await response.Content.ReadAsStringAsync();
        allPosts = JsonSerializer.Deserialize<PostIndex[]>(json, options: new() { PropertyNameCaseInsensitive = true }) ?? [];
        Posts = allPosts;
    }
    private async Task ReloadPosts(bool ignoreCache)
    {
        isLoading = true;
        await LoadAsync(ignoreCache);
        isLoading = false;
        ApplySearchQuery();
        // TODO: Handle fetch failure
    }

    #endregion

    #region Search

    private const string TagSearchPrefix = "Tag:";
    private const string CatSearchPrefix = "Cat:";

    private void ApplySearchQuery()
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            Posts = allPosts;
            CurrentPage = 1;
            return;
        }

        if (searchQuery.StartsWith(TagSearchPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var tagQuery = searchQuery[TagSearchPrefix.Length..].Trim();
            Posts = allPosts
                .Where(p => p.Tags?.Any(t => t.Contains(tagQuery, StringComparison.OrdinalIgnoreCase)) == true)
                .OrderByDescending(p => p.Date)
                .ToArray();
        }
        else if (searchQuery.StartsWith(CatSearchPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var catQuery = searchQuery[CatSearchPrefix.Length..].Trim();
            Posts = allPosts
                .Where(p => p.Category?.Contains(catQuery, StringComparison.OrdinalIgnoreCase) == true)
                .OrderByDescending(p => p.Date)
                .ToArray();
        }
        else
        {
            Posts = allPosts.Where(p =>
                p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                p.ShortDescription?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) == true ||
                (p.Tags?.Any(t => t.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)) ?? false))
                .OrderByDescending(p => p.Date)
                .ToArray();
        }

        CurrentPage = 1;
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
