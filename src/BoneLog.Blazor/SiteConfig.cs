namespace BoneLog.Blazor.Dtos;


public record NavItemDto(string Title,string Url);
public record SocialLinkDto(string Url,string IconClass);
public record class AboutMeDto(string Name, string? Headline, string? Avatar, string? ContentUrl);

public record SiteConfig(
    string Title,
    string PostsPath,
    string IndexPath,
    string CategoriesPath,
    AboutMeDto AboutMe,
    List<NavItemDto>? NavItems,
    List<SocialLinkDto>? SocialLinks);

