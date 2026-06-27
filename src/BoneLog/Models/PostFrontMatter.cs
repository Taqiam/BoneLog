using System.Globalization;
using BoneLog.Tools;

namespace BoneLog.Models;

public class PostFrontMatter
{
    public string? Id { get; set; }
    public string? Slug { get; set; }
    public string? CategoryPath { get; set; }
    public string? Title { get; set; }
    public string? Date { get; set; }
    public string[]? Tags { get; set; }
    public string? Cover { get; set; }
    public string? Thumbnail { get; set; }
    public string? ShortDescription { get; set; }

    public static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt) ? dt : null;
    }

    public static PostFrontMatter MergeForLanguage(PostFrontMatter? english, PostFrontMatter? localized, string language)
    {
        if (english is null)
            return localized ?? new PostFrontMatter();

        if (localized is null || language.Equals(PathExtensions.DefaultLanguage, StringComparison.OrdinalIgnoreCase))
            return english;

        return new PostFrontMatter
        {
            Id = english.Id,
            Slug = english.Slug,
            CategoryPath = english.CategoryPath,
            Date = english.Date,
            Tags = english.Tags,
            ShortDescription = english.ShortDescription,
            Thumbnail = english.Thumbnail,
            Cover = english.Cover,
            Title = string.IsNullOrWhiteSpace(localized.Title) ? english.Title : localized.Title
        };
    }
}
