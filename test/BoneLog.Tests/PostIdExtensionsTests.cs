namespace BoneLog.Tests;

public class PostIdExtensionsTests
{
    [Theory]
    [InlineData("000001", true)]
    [InlineData("1000", true)]
    [InlineData("0", true)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData("not-numeric", false)]
    [InlineData("00000A", false)]
    [InlineData("a1b2", false)]
    public void IsPostId_ValidatesNumericIds(string? value, bool expected) =>
        Assert.Equal(expected, value.IsPostId());

    [Theory]
    [InlineData("  000006  ", "000006")]
    [InlineData("42", "42")]
    public void TryNormalizePostId_TrimsWhitespace(string input, string expected)
    {
        Assert.True(input.TryNormalizePostId(out var normalized));
        Assert.Equal(expected, normalized);
    }

    [Fact]
    public void PostIdEquals_MatchesExactNumericForm()
    {
        Assert.True("000001".PostIdEquals("000001"));
        Assert.False("000001".PostIdEquals("000002"));
        Assert.False("000001".PostIdEquals("1"));
    }

    [Fact]
    public void ComparePostIds_OrdersNumerically()
    {
        Assert.True(PostIdExtensions.ComparePostIds("000002", "000010") < 0);
        Assert.True(PostIdExtensions.ComparePostIds("000010", "000002") > 0);
        Assert.Equal(0, PostIdExtensions.ComparePostIds("000006", "000006"));
    }
}
