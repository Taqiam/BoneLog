using BoneLog.Models;

namespace BoneLog.Abstractions;

public interface IPostReader
{
    Task<Post?> Get(string relativePath, bool ignoreCache = false);
    Task<PostIndex[]> GetIndex(bool ignoreCache = false);
    Task<Category[]> GetCategories(bool ignoreCache = false);
    Task<AboutMe?> GetAboutMe(bool ignoreCache = false);
}