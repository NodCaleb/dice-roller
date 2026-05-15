using DiceRoller.Models;
using DiceRoller.Services;
using FakeItEasy;

namespace DiceRoller.Tests.Rolling;

public class DiceRollerServiceTests
{
    private readonly IRandomSource _random = A.Fake<IRandomSource>();
    private readonly DiceRollerService _sut;

    public DiceRollerServiceTests()
    {
        _sut = new DiceRollerService(_random);
    }

    [Fact]
    public void Roll_ReturnsDiceValuesCountMatchingExpression()
    {
        var expr = new DiceExpression(3, DieType.D8, 0);
        A.CallTo(() => _random.Next(1, 8)).Returns(4);

        var result = _sut.Roll(expr);

        Assert.Equal(3, result.DiceValues.Count);
    }

    [Fact]
    public void Roll_EachDieValueWithinRange()
    {
        var expr = new DiceExpression(5, DieType.D6, 0);
        A.CallTo(() => _random.Next(1, 6)).ReturnsNextFromSequence(1, 2, 3, 4, 5);

        var result = _sut.Roll(expr);

        Assert.All(result.DiceValues, v => Assert.InRange(v, 1, 6));
    }

    [Fact]
    public void Roll_TotalEqualsDiceSumPlusModifier()
    {
        var expr = new DiceExpression(3, DieType.D8, 2);
        A.CallTo(() => _random.Next(1, 8)).ReturnsNextFromSequence(4, 3, 5);

        var result = _sut.Roll(expr);

        // 4 + 3 + 5 + 2 = 14
        Assert.Equal(14, result.Total);
    }

    [Fact]
    public void Roll_ZeroModifierDoesNotChangeTotal()
    {
        var expr = new DiceExpression(2, DieType.D6, 0);
        A.CallTo(() => _random.Next(1, 6)).ReturnsNextFromSequence(3, 4);

        var result = _sut.Roll(expr);

        Assert.Equal(7, result.Total);
    }

    [Fact]
    public void Roll_PositiveModifierIsAdded()
    {
        var expr = new DiceExpression(1, DieType.D20, 5);
        A.CallTo(() => _random.Next(1, 20)).Returns(10);

        var result = _sut.Roll(expr);

        Assert.Equal(15, result.Total);
    }

    [Fact]
    public void Roll_NegativeModifierIsSubtracted()
    {
        var expr = new DiceExpression(2, DieType.D6, -1);
        A.CallTo(() => _random.Next(1, 6)).ReturnsNextFromSequence(3, 3);

        var result = _sut.Roll(expr);

        // 3 + 3 - 1 = 5
        Assert.Equal(5, result.Total);
    }

    [Fact]
    public void Roll_SingleD20NoModifier_TotalEqualsDieValue()
    {
        var expr = new DiceExpression(1, DieType.D20, 0);
        A.CallTo(() => _random.Next(1, 20)).Returns(17);

        var result = _sut.Roll(expr);

        Assert.Equal(17, result.Total);
        Assert.Equal(17, result.DiceValues[0]);
    }

    [Fact]
    public void Roll_UsesRandomSourceWithCorrectRange()
    {
        var expr = new DiceExpression(1, DieType.D12, 0);
        A.CallTo(() => _random.Next(1, 12)).Returns(6);

        _sut.Roll(expr);

        A.CallTo(() => _random.Next(1, 12)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Roll_ExpressionPreservedInResult()
    {
        var expr = new DiceExpression(2, DieType.D4, 3);
        A.CallTo(() => _random.Next(1, 4)).Returns(2);

        var result = _sut.Roll(expr);

        Assert.Equal(expr, result.Expression);
    }
}
