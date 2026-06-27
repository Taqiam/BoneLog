namespace BoneLog.Tests;

public class SearchQueryPartsTests
{
    [Fact]
    public void Parse_EmptyQuery_ReturnsEmpty()
    {
        var result = SearchQueryParser.Parse("");
        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void Parse_TagCategoryAndFreeText_ExtractsAllParts()
    {
        var result = SearchQueryParser.Parse("hello Tag:dotnet Cat:Dev");

        Assert.Equal(["dotnet"], result.Tags);
        Assert.Equal(["Dev"], result.Categories);
        Assert.Equal(["hello"], result.FreeText);
        Assert.Empty(result.Languages);
    }

    [Fact]
    public void Parse_Language_ExtractsLanguage()
    {
        var result = SearchQueryParser.Parse("Lang:EN");

        Assert.Equal(["EN"], result.Languages);
        Assert.Empty(result.FreeText);
    }

    [Fact]
    public void Parse_LanguageWithFreeText_ExtractsBoth()
    {
        var result = SearchQueryParser.Parse("hello Lang:EN");

        Assert.Equal(["EN"], result.Languages);
        Assert.Equal(["hello"], result.FreeText);
    }

    [Fact]
    public void Parse_LanguageBeforeFreeText_ExtractsBoth()
    {
        var result = SearchQueryParser.Parse("Lang:EN hello");

        Assert.Equal(["EN"], result.Languages);
        Assert.Equal(["hello"], result.FreeText);
    }
}
