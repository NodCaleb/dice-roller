namespace DiceRoller.Models;

public record RollResult(DiceExpression Expression, IReadOnlyList<int> DiceValues)
{
    public int Total => DiceValues.Sum() + Expression.Modifier;
}
