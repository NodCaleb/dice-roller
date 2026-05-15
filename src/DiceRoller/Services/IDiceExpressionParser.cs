using DiceRoller.Models;

namespace DiceRoller.Services;

public interface IDiceExpressionParser
{
    ParseResult<DiceExpression> Parse(string input);
}
