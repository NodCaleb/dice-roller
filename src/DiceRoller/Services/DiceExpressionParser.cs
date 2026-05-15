using System.Text.RegularExpressions;
using DiceRoller.Models;

namespace DiceRoller.Services;

public sealed partial class DiceExpressionParser : IDiceExpressionParser
{
    [GeneratedRegex(@"^(?<count>\d+)?[dD](?<sides>\d+)(?<modifier>[+-]\d+)?$", RegexOptions.Compiled)]
    private static partial Regex ExpressionRegex();

    public ParseResult<DiceExpression> Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return ParseResult<DiceExpression>.Fail(
                "Invalid dice expression. Expected format: 1d20, 3d8+2, d6-1");

        var match = ExpressionRegex().Match(input.Trim());
        if (!match.Success)
            return ParseResult<DiceExpression>.Fail(
                "Invalid dice expression. Expected format: 1d20, 3d8+2, d6-1");

        // Parse sides
        if (!int.TryParse(match.Groups["sides"].Value, out int sides))
            return ParseResult<DiceExpression>.Fail(
                "Invalid dice expression. Expected format: 1d20, 3d8+2, d6-1");

        if (!DieTypeExtensions.ValidSides.Contains(sides))
            return ParseResult<DiceExpression>.Fail(
                $"Unsupported die type 'd{sides}'. Valid types: d4, d6, d8, d10, d12, d20, d100");

        // Parse count (default 1)
        int count = 1;
        if (match.Groups["count"].Success)
        {
            if (!int.TryParse(match.Groups["count"].Value, out count))
                return ParseResult<DiceExpression>.Fail("Invalid dice count.");
        }

        if (count < 1 || count > 20)
            return ParseResult<DiceExpression>.Fail("Dice count must be between 1 and 20");

        // Parse modifier (default 0)
        int modifier = 0;
        if (match.Groups["modifier"].Success)
        {
            if (!int.TryParse(match.Groups["modifier"].Value, out modifier))
                return ParseResult<DiceExpression>.Fail("Invalid modifier value.");
        }

        if (modifier < -20 || modifier > 20)
            return ParseResult<DiceExpression>.Fail("Modifier must be between −20 and +20");

        var dieType = (DieType)sides;
        return ParseResult<DiceExpression>.Ok(new DiceExpression(count, dieType, modifier));
    }
}
