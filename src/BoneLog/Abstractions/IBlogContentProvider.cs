using BoneLog.Models;

namespace BoneLog.Abstractions;

public interface IBlogContentProvider
{
    Task<PostIndex?> GetPostIndexEntry(string id, bool ignoreCache = false);
    Task<Post?> GetPostByIdAndLanguage(string id, string language, bool ignoreCache = false);
    Task<PostIndex[]> GetIndex(bool ignoreCache = false);
    Task<AboutMe?> GetAboutMe(bool ignoreCache = false);
    Task<string?> GetContent(string relativePath, bool ignoreCache = false);
}
