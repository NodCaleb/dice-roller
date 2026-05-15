using DiceRoller.Models;

namespace DiceRoller.Services;

public interface IDiceRoller
{
    RollResult Roll(DiceExpression expression);
}
