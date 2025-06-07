using BoneLog.Blazor.Dtos;
using BoneLog.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;

namespace BoneLog.Blazor.Pages;

public partial class Home : ComponentBase
{
    [Inject] HttpClient Http { get; set; } = default!;
    [Inject] NavigationManager Nav { get; set; } = default!;
    [Inject] SiteSettingsService SiteSettings { get; set; } = default!;

    private List<PostIndexDto>? posts;
    private List<PostIndexDto>? filteredPosts;
    private int currentPage = 1;
    private int pageCount = 0;
    private const int itemsPerPage = 10;

    private string searchQuery = "";

    protected override async Task OnInitializedAsync()
    {
        await SiteSettings.LoadAsync();
        var json = await Http.GetStringAsync($"data/posts.json");

        posts = JsonSerializer.Deserialize<List<PostIndexDto>>(json,new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        });

        if(posts != null)
        {
            FilterPosts();
        }
    }

    protected override void OnParametersSet()
    {
        var uri = Nav.ToAbsoluteUri(Nav.Uri);
        if(QueryHelpers.ParseQuery(uri.Query).TryGetValue("search",out var searchValue))
        {
            searchQuery = searchValue.ToString();
            FilterPosts();
        }
    }

    private void FilterPosts()
    {
        if(string.IsNullOrWhiteSpace(searchQuery))
        {
            filteredPosts = posts;
        }
        else
        {
            var query = searchQuery.Trim().ToLower();
            if(query.StartsWith("#"))
            {
                query = query.Substring(1);
                filteredPosts = posts?.Where(p => p.Tags?.Any(tag => tag.Contains(query,StringComparison.OrdinalIgnoreCase)) ?? false).ToList();
            }
            filteredPosts = posts?.Where(p =>
                    (p.Title.ToLower().Contains(query)) ||
                    (p.ShortDescription?.ToLower().Contains(query) ?? false) ||
                    (p.Tags?.Any(tag => tag.Contains(query,StringComparison.OrdinalIgnoreCase)) ?? false) ||
                    (p.Date.ToString().Contains(query))).ToList();
        }

        filteredPosts = filteredPosts?.OrderByDescending(p => p.Date).ToList();
        pageCount = (int)Math.Ceiling((double)(filteredPosts?.Count ?? 0) / itemsPerPage);
        currentPage = 1;
    }

    private void NextPage()
    {
        if(currentPage < pageCount)
        {
            currentPage++;
        }
    }

    private void PrevPage()
    {
        if(currentPage > 1)
        {
            currentPage--;
        }
    }

    private void OnPageInputChanged(ChangeEventArgs e)
    {
        if(int.TryParse(e.Value?.ToString(),out int val))
        {
            if(val >= 1 && val <= pageCount)
            {
                currentPage = val;
            }
        }
    }

    private void OnSearchInput(ChangeEventArgs e)
    {
        searchQuery = e.Value?.ToString() ?? "";
        FilterPosts();
    }
    private async Task ReloadPosts()
    {
        var url = $"data/posts.json?nocache={Guid.NewGuid()}";
        var json = await Http.GetStringAsync(url);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        posts = JsonSerializer.Deserialize<List<PostIndexDto>>(json,options);
        FilterPosts();
    }
    private void ClearSearch()
    {
        searchQuery = "";
        FilterPosts();
    }
}
