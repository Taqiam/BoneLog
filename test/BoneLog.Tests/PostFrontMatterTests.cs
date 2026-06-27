namespace BoneLog.Tests;

public class PostFrontMatterTests
{
    [Fact]
    public void MergeForLanguage_UsesEnglishMetadataAndLocalizedTitle()
    {
        var english = new PostFrontMatter
        {
            Id = "001",
            Slug = "hello",
            CategoryPath = "Guide",
            Title = "English title",
            Tags = ["docs"],
            ShortDescription = "Summary"
        };

        var localized = new PostFrontMatter
        {
            Title = "Persian title"
        };

        var merged = PostFrontMatter.MergeForLanguage(english, localized, "fa");

        Assert.Equal("001", merged.Id);
        Assert.Equal("Guide", merged.CategoryPath);
        Assert.Equal("Persian title", merged.Title);
        Assert.Equal("docs", merged.Tags!.Single());
        Assert.Equal("Summary", merged.ShortDescription);
    }

    [Fact]
    public void MergeForLanguage_FallsBackToEnglishTitleWhenLocalizedMissing()
    {
        var english = new PostFrontMatter { Title = "English title", CategoryPath = "Guide" };

        var merged = PostFrontMatter.MergeForLanguage(english, new PostFrontMatter(), "fa");

        Assert.Equal("English title", merged.Title);
        Assert.Equal("Guide", merged.CategoryPath);
    }
}
