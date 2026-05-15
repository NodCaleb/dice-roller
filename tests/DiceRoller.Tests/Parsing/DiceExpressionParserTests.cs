using DiceRoller.Models;
using DiceRoller.Services;

namespace DiceRoller.Tests.Parsing;

public class DiceExpressionParserTests
{
    private readonly DiceExpressionParser _sut = new();

    // ── Valid expressions ────────────────────────────────────────────────────

    [Fact]
    public void Parse_d20_ReturnsCount1_DieD20_Modifier0()
    {
        var result = _sut.Parse("d20");
        AssertSuccess(result, 1, DieType.D20, 0);
    }

    [Fact]
    public void Parse_1d20_ReturnsCount1_DieD20_Modifier0()
    {
        var result = _sut.Parse("1d20");
        AssertSuccess(result, 1, DieType.D20, 0);
    }

    [Fact]
    public void Parse_3d8Plus2_ReturnsCorrectExpression()
    {
        var result = _sut.Parse("3d8+2");
        AssertSuccess(result, 3, DieType.D8, 2);
    }

    [Fact]
    public void Parse_2d6Minus1_ReturnsNegativeModifier()
    {
        var result = _sut.Parse("2d6-1");
        AssertSuccess(result, 2, DieType.D6, -1);
    }

    [Fact]
    public void Parse_UppercaseD_IsCaseInsensitive()
    {
        var result = _sut.Parse("D20");
        AssertSuccess(result, 1, DieType.D20, 0);
    }

    [Fact]
    public void Parse_1d20Plus0_ZeroModifierAllowed()
    {
        var result = _sut.Parse("1d20+0");
        AssertSuccess(result, 1, DieType.D20, 0);
    }

    [Fact]
    public void Parse_MaxCount20_IsAccepted()
    {
        var result = _sut.Parse("20d4");
        AssertSuccess(result, 20, DieType.D4, 0);
    }

    [Fact]
    public void Parse_ModifierMinus20_IsAccepted()
    {
        var result = _sut.Parse("1d20-20");
        AssertSuccess(result, 1, DieType.D20, -20);
    }

    [Fact]
    public void Parse_ModifierPlus20_IsAccepted()
    {
        var result = _sut.Parse("1d20+20");
        AssertSuccess(result, 1, DieType.D20, 20);
    }

    [Fact]
    public void Parse_1d100_IsAccepted()
    {
        var result = _sut.Parse("1d100");
        AssertSuccess(result, 1, DieType.D100, 0);
    }

    // ── Invalid: unsupported die type ────────────────────────────────────────

    [Fact]
    public void Parse_d7_ReturnsFailureUnsupportedDieType()
    {
        var result = _sut.Parse("d7");
        AssertFailure(result, "d7");
    }

    [Fact]
    public void Parse_5d0_ReturnsFailure()
    {
        var result = _sut.Parse("5d0");
        AssertFailure(result);
    }

    // ── Invalid: count out of range ──────────────────────────────────────────

    [Fact]
    public void Parse_0d6_ReturnsFailureCountTooLow()
    {
        var result = _sut.Parse("0d6");
        AssertFailure(result);
    }

    [Fact]
    public void Parse_21d6_ReturnsFailureCountTooHigh()
    {
        var result = _sut.Parse("21d6");
        AssertFailure(result);
    }

    // ── Invalid: modifier out of range ───────────────────────────────────────

    [Fact]
    public void Parse_1d6Plus99_ReturnsFailureModifierTooHigh()
    {
        var result = _sut.Parse("1d6+99");
        AssertFailure(result);
    }

    [Fact]
    public void Parse_1d6Minus99_ReturnsFailureModifierTooLow()
    {
        var result = _sut.Parse("1d6-99");
        AssertFailure(result);
    }

    // ── Invalid: format errors ───────────────────────────────────────────────

    [Fact]
    public void Parse_abc_ReturnsFailureInvalidFormat()
    {
        var result = _sut.Parse("abc");
        AssertFailure(result);
    }

    [Fact]
    public void Parse_WithEmbeddedWhitespace_ReturnsFailure()
    {
        var result = _sut.Parse("1d20 +5");
        AssertFailure(result);
    }

    [Fact]
    public void Parse_DoubleSign_ReturnsFailure()
    {
        var result = _sut.Parse("1d20++5");
        AssertFailure(result);
    }

    [Fact]
    public void Parse_EmptyString_ReturnsFailure()
    {
        var result = _sut.Parse(string.Empty);
        AssertFailure(result);
    }

    // ── Canonical form ───────────────────────────────────────────────────────

    [Fact]
    public void Parse_d20_CanonicalIs1d20()
    {
        var result = _sut.Parse("d20");
        Assert.Equal("1d20", result.Value!.ToCanonical());
    }

    [Fact]
    public void Parse_1d20Plus0_CanonicalOmitsZeroModifier()
    {
        var result = _sut.Parse("1d20+0");
        Assert.Equal("1d20", result.Value!.ToCanonical());
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static void AssertSuccess(
        ParseResult<DiceExpression> result, int count, DieType die, int modifier)
    {
        Assert.True(result.IsSuccess, $"Expected success but got: {result.ErrorMessage}");
        Assert.NotNull(result.Value);
        Assert.Equal(count, result.Value!.Count);
        Assert.Equal(die, result.Value.Die);
        Assert.Equal(modifier, result.Value.Modifier);
    }

    private static void AssertFailure(
        ParseResult<DiceExpression> result, string? containsText = null)
    {
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.ErrorMessage);
        if (containsText is not null)
            Assert.Contains(containsText, result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }
}
