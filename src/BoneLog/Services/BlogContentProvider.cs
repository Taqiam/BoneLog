using BoneLog.Abstractions;
using BoneLog.Models;
using BoneLog.Tools;
using System.Text.Json;

namespace BoneLog.Services;

public class BlogContentProvider(HttpClient httpClient, PathSettings pathSettings) : IBlogContentProvider
{
    public async Task<Post?> GetPost(string relativePath, bool ignoreCache = false)
    {
        var fullPath = pathSettings.GetPostsPath().ToMarkdownFetchPath(relativePath).ApplyIgnoreCache(ignoreCache);
        var response = await httpClient.GetAsync(fullPath);

        if (!response.IsSuccessStatusCode)
            return null;

        string markdown = await response.Content.ReadAsStringAsync();
        var normalizedPath = relativePath.NormalizeRelativePath();
        return markdown.ToPost(normalizedPath);
    }

    public async Task<PostIndex[]> GetIndex(bool ignoreCache = false)
    {
        var path = pathSettings.GetIndexPath().ApplyIgnoreCache(ignoreCache);
        var response = await httpClient.GetAsync(path);

        if (!response.IsSuccessStatusCode)
            return [];

        string json = await response.Content.ReadAsStringAsync();
        PostIndex[]? posts = JsonSerializer.Deserialize<PostIndex[]>(json, options: new() { PropertyNameCaseInsensitive = true });

        return posts ?? [];
    }

    public async Task<AboutMe?> GetAboutMe(bool ignoreCache = false)
    {
        var response = await httpClient.GetAsync(pathSettings.GetAboutMePath().ApplyIgnoreCache(ignoreCache));

        if (!response.IsSuccessStatusCode)
            return null;

        string markdown = await response.Content.ReadAsStringAsync();
        return markdown.ToAboutMe();
    }

    public async Task<string?> GetContent(string relativePath, bool ignoreCache = false)
    {
        var fullPath = pathSettings.BaseDataPath.ToMarkdownFetchPath(relativePath).ApplyIgnoreCache(ignoreCache);
        var response = await httpClient.GetAsync(fullPath);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        return content.ToHtmlBody();
    }
}
