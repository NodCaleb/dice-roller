using DiceRoller.Models;

namespace DiceRoller.Services;

public sealed class DiceRollerService : IDiceRoller
{
    private readonly IRandomSource _random;

    public DiceRollerService(IRandomSource random)
    {
        _random = random;
    }

    public RollResult Roll(DiceExpression expression)
    {
        var sides = (int)expression.Die;
        var values = Enumerable
            .Range(0, expression.Count)
            .Select(_ => _random.Next(1, sides))
            .ToList();

        return new RollResult(expression, values);
    }
}
