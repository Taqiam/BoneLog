using BoneLog.Models;

namespace BoneLog.Abstractions;

public interface IPostReader
{
    Task<Post?> GetPost(string relativePath, bool ignoreCache = false);
    Task<PostIndex[]> GetIndex(bool ignoreCache = false);
    Task<Category[]> GetCategories(bool ignoreCache = false);
    Task<AboutMe?> GetAboutMe(bool ignoreCache = false);
    Task<string?> GetContent(string relativePath, bool ignoreCache = false);
}