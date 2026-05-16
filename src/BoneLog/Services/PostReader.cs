using BoneLog.Abstractions;
using BoneLog.Tools;
using BoneLog.Models;
using System.Text.Json;

namespace BoneLog.Services;

public class PostReader(HttpClient httpClient, PathSettings pathSettings) : IPostReader
{
    public async Task<Post?> Get(string relativePath, bool ignoreCache = false)
    {
        string fullPath = Path.Combine(pathSettings.PostsPath, $"{relativePath}.md");
        var response = await httpClient.GetAsync(fullPath);

        if(!response.IsSuccessStatusCode)
            return null;

        string markdown = await response.Content.ReadAsStringAsync();

        (Post? result, string htmlContent) = markdown.ParseMarkdownToHtmlWithHeader<Post>();

        if (result != null)
        {
            result.Content = htmlContent;
        }
        else
        {
            result = new("Untitled Post", relativePath, htmlContent);
        }

        return result;
    }

    public async Task<PostIndex[]> GetIndex(bool ignoreCache = false)
    {
        var response = await httpClient.GetAsync(pathSettings.IndexPath);

        if (!response.IsSuccessStatusCode)
            return [];

        string json = await response.Content.ReadAsStringAsync();
        PostIndex[]? posts = JsonSerializer.Deserialize<PostIndex[]>(json, options: new() { PropertyNameCaseInsensitive = true });

        return posts ?? [];
    }

    public async Task<Category[]> GetCategories(bool ignoreCache = false)
    {
        var response = await httpClient.GetAsync(pathSettings.CategoriesPath);

        if (!response.IsSuccessStatusCode)
            return [];

        string json = await response.Content.ReadAsStringAsync();
        Category[]? categories = JsonSerializer.Deserialize<Category[]>(json, options: new() { PropertyNameCaseInsensitive = true });

        return categories ?? [];
    }
}
