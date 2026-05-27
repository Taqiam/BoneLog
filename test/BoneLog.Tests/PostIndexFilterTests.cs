namespace BoneLog.Tests;

public class PostIndexFilterTests
{
    private static PostIndex Post(
        string title,
        string? category = null,
        string[]? tags = null,
        string language = "EN",
        DateTime? date = null) =>
        new()
        {
            Title = title,
            Path = title.ToLowerInvariant(),
            Category = category,
            Tags = tags,
            Language = language,
            Date = date,
        };

    [Fact]
    public void Apply_EmptyParsed_ReturnsAllPostsUnsorted()
    {
        var posts = new[]
        {
            Post("B", date: new DateTime(2020, 1, 1)),
            Post("A", date: new DateTime(2021, 1, 1)),
        };

        var result = posts.ApplySearch(SearchQueryParser.Empty);

        Assert.Equal(2, result.Length);
        Assert.Equal("B", result[0].Title);
    }

    [Fact]
    public void Apply_TagFilter_MatchesPostsWithTag()
    {
        var posts = new[]
        {
            Post("With", tags: ["dotnet"]),
            Post("Without", tags: ["other"]),
        };
        var parsed = SearchQueryParser.Parse("Tag:dotnet", enableLanguage: true);

        var result = posts.ApplySearch(parsed);

        Assert.Single(result);
        Assert.Equal("With", result[0].Title);
    }

    [Fact]
    public void Apply_WithFilters_OrdersByDateDescending()
    {
        var posts = new[]
        {
            Post("Old", language: "EN", date: new DateTime(2020, 1, 1)),
            Post("New", language: "EN", date: new DateTime(2022, 1, 1)),
        };
        var parsed = SearchQueryParser.Parse("Lang:EN", enableLanguage: true);

        var result = posts.ApplySearch(parsed);

        Assert.Equal("New", result[0].Title);
        Assert.Equal("Old", result[1].Title);
    }
}
