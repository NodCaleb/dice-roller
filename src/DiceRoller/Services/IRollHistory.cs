using DiceRoller.Models;

namespace DiceRoller.Services;

public interface IRollHistory
{
    IReadOnlyList<DiceExpression> Entries { get; }
    void Add(DiceExpression expression);
}
