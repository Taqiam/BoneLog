using BoneLog.Abstractions;
using BoneLog.Models;
using BoneLog.Tools;
using System.Text.Json;

namespace BoneLog.Services;

public class PostReader(HttpClient httpClient, PathSettings pathSettings) : IPostReader
{
    public async Task<Post?> GetPost(string relativePath, bool ignoreCache = false)
    {
        var postsBase = pathSettings.GetPostsPath().TrimEnd('/');
        var normalizedPath = relativePath.TrimStart('/').Replace('\\', '/');
        var fullPath = $"{postsBase}/{normalizedPath}.md";

        var response = await httpClient.GetAsync(fullPath);

        if (!response.IsSuccessStatusCode)
            return null;

        string markdown = await response.Content.ReadAsStringAsync();
        var (frontMatter, htmlContent) = markdown.ParseMarkdownToHtmlWithFrontMatter();
        var category = await ResolveCategoryAsync(normalizedPath, ignoreCache);

        return Post.Create(normalizedPath, htmlContent, frontMatter, category);
    }

    public async Task<PostIndex[]> GetIndex(bool ignoreCache = false)
    {
        var path = pathSettings.GetIndexPath() + (ignoreCache ? $"?nocache={Guid.NewGuid()}" : "");
        var response = await httpClient.GetAsync(path);

        if (!response.IsSuccessStatusCode)
            return [];

        string json = await response.Content.ReadAsStringAsync();
        PostIndex[]? posts = JsonSerializer.Deserialize<PostIndex[]>(json, options: new() { PropertyNameCaseInsensitive = true });

        return posts ?? [];
    }

    public async Task<Category[]> GetCategories(bool ignoreCache = false)
    {
        var response = await httpClient.GetAsync(pathSettings.GetCategoriesPath());

        if (!response.IsSuccessStatusCode)
            return [];

        string json = await response.Content.ReadAsStringAsync();
        Category[]? categories = JsonSerializer.Deserialize<Category[]>(json, options: new() { PropertyNameCaseInsensitive = true });

        return categories ?? [];
    }

    public async Task<AboutMe?> GetAboutMe(bool ignoreCache = false)
    {
        var response = await httpClient.GetAsync(pathSettings.GetAboutMePath());

        if (!response.IsSuccessStatusCode)
            return null;

        string markdown = await response.Content.ReadAsStringAsync();
        var (frontMatter, htmlContent) = markdown.ParseMarkdownToHtmlWithHeader<AboutMeFrontMatter>();

        return AboutMe.Create(frontMatter, htmlContent);
    }

    private async Task<string?> ResolveCategoryAsync(string path, bool ignoreCache)
    {
        var index = await GetIndex(ignoreCache);
        var entry = index.FirstOrDefault(p =>
            string.Equals(p.Path, path, StringComparison.OrdinalIgnoreCase));

        return entry?.Category ?? PostPathHelper.CategoryFromPath(path);
    }

    public async Task<string?> GetContent(string relativePath, bool ignoreCache = false)
    {

        var basePath = pathSettings.BaseDataPath.TrimEnd('/');
        var normalizedPath = relativePath.TrimStart('/').Replace('\\', '/');
        var fullPath = $"{basePath}/{normalizedPath}.md";

        var response = await httpClient.GetAsync(fullPath);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        string htmlContent = content.RemoveYamlHeader().MarkdownToHtml();
        return htmlContent;
    }
}