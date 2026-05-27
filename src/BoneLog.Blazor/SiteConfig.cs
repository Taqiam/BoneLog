using BoneLog.Models;
using BoneLog.Tools;

namespace BoneLog.Blazor.Dtos;

public record NavItemDto(string Title, string Url);
public record SocialLinkDto(string Url, string IconClass);

public record SiteFeatures(
    bool CategorySidebar = true,
    bool LanguageSidebar = true,
    bool TagSidebar = true,
    bool EnableMultilanguage = true);

public record SiteConfig(
    string Title,
    PathSettings Paths,
    NavItemDto[]? NavItems,
    SocialLinkDto[]? SocialLinks,
    int PostsPerPage = 10,
    SiteFeatures? Features = null,
    string BaseDir = "/")
{
    public SiteFeatures FeaturesOrDefault => Features ?? new SiteFeatures();

    public string NormalizedBaseDir => ContentPathExtensions.NormalizeBaseDir(BaseDir);
}
