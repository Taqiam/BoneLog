using BoneLog.Models;

namespace BoneLog.Blazor.Dtos;

public record NavItemDto(string Title, string Url);
public record SocialLinkDto(string Url, string IconClass);

public record SiteFeatures(bool CategorySidebar = true);

public record SiteConfig(
    string Title,
    PathSettings Paths,
    List<NavItemDto>? NavItems,
    List<SocialLinkDto>? SocialLinks,
    SiteFeatures? Features = null)
{
    public SiteFeatures FeaturesOrDefault => Features ?? new SiteFeatures();
}
