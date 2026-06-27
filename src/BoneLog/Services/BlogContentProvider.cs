using BoneLog.Abstractions;
using BoneLog.Models;
using BoneLog.Tools;
using System.Text.Json;

namespace BoneLog.Services;

public class BlogContentProvider(HttpClient httpClient, PathSettings pathSettings) : IBlogContentProvider
{
    public async Task<Post?> GetPostByIdAndLanguage(string id, string language, bool ignoreCache = false)
    {
        if (!id.TryNormalizePostId(out var normalizedId))
            return null;

        var index = await GetIndex(ignoreCache);
        var entry = index.FirstOrDefault(p => p.Id.PostIdEquals(normalizedId));

        if (entry is null || !entry.FilePaths.TryGetValue(language, out var filePath))
            return null;

        entry.FilePaths.TryGetValue(PathExtensions.DefaultLanguage, out var englishFilePath);
        return await LoadPostFile(filePath, englishFilePath, language, ignoreCache);
    }

    public async Task<PostIndex?> GetPostIndexEntry(string id, bool ignoreCache = false)
    {
        if (!id.TryNormalizePostId(out var normalizedId))
            return null;

        var index = await GetIndex(ignoreCache);
        return index.FirstOrDefault(p => p.Id.PostIdEquals(normalizedId));
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
        var aboutPath = pathSettings.AboutMePath.NormalizeRelativePath();
        return markdown.ToAboutMe(pathSettings, aboutPath);
    }

    public async Task<string?> GetContent(string relativePath, bool ignoreCache = false)
    {
        var fullPath = pathSettings.BaseDataPath.ToMarkdownFetchPath(relativePath).ApplyIgnoreCache(ignoreCache);
        var response = await httpClient.GetAsync(fullPath);

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        var normalizedPath = relativePath.NormalizeRelativePath();
        return content.ToHtmlBody(pathSettings);
    }

    private async Task<Post?> LoadPostFile(string filePath, string? englishFilePath, string language, bool ignoreCache)
    {
        var markdown = await FetchMarkdown(pathSettings.GetPostsPath().ToMarkdownFetchPath(filePath), ignoreCache);
        if (markdown is null)
            return null;

        PostFrontMatter? englishFrontMatter = null;
        if (!string.IsNullOrWhiteSpace(englishFilePath)
            && !englishFilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
        {
            var englishMarkdown = await FetchMarkdown(pathSettings.GetPostsPath().ToMarkdownFetchPath(englishFilePath), ignoreCache);
            if (englishMarkdown is not null)
                englishFrontMatter = englishMarkdown.ToHtmlWithPostFrontMatter().Item1;
        }

        var normalizedPath = filePath.NormalizeRelativePath();
        return markdown.ToPost(normalizedPath, pathSettings, englishFrontMatter, language);
    }

    private async Task<string?> FetchMarkdown(string url, bool ignoreCache)
    {
        var response = await httpClient.GetAsync(url.ApplyIgnoreCache(ignoreCache));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync();
    }
}
