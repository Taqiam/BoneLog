namespace BoneLog.Tests;

public class SearchQueryPartsTests
{
    [Fact]
    public void Parse_EmptyQuery_ReturnsEmpty()
    {
        var result = SearchQueryParser.Parse("", enableLanguage: true);
        Assert.True(result.IsEmpty);
    }

    [Fact]
    public void Parse_TagCategoryAndFreeText_ExtractsAllParts()
    {
        var result = SearchQueryParser.Parse("hello Tag:dotnet Cat:Dev", enableLanguage: true);

        Assert.Equal(["dotnet"], result.Tags);
        Assert.Equal(["Dev"], result.Categories);
        Assert.Equal(["hello"], result.FreeText);
        Assert.True(result.Languages.Count == 0);
    }

    [Fact]
    public void Parse_LanguageDisabled_TreatsLangAsFreeText()
    {
        var result = SearchQueryParser.Parse("Lang:EN hello", enableLanguage: false);

        Assert.Empty(result.Languages);
        Assert.Equal(["Lang:EN", "hello"], result.FreeText);
    }

    [Fact]
    public void Parse_LanguageEnabled_ExtractsLanguage()
    {
        var result = SearchQueryParser.Parse("Lang:EN", enableLanguage: true);

        Assert.Equal(["EN"], result.Languages);
    }
}
