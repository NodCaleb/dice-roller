using DiceRoller.Services;
using DiceRoller.Ui;
using Spectre.Console;

namespace DiceRoller.Ui;

public sealed class ExpressionEntryPrompt
{
    private readonly IDiceExpressionParser _parser;
    private readonly IDiceRoller _roller;
    private readonly IRollHistory? _history;

    public ExpressionEntryPrompt(
        IDiceExpressionParser parser,
        IDiceRoller roller,
        IRollHistory? history = null)
    {
        _parser = parser;
        _roller = roller;
        _history = history;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.WriteLine();
            var input = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter a dice expression [grey](e.g. 3d8+2, d20, 2d6-1)[/]:"));

            var parsed = _parser.Parse(input);
            if (!parsed.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]✗ Error:[/] {Markup.Escape(parsed.ErrorMessage!)}");
                continue;
            }

            var result = _roller.Roll(parsed.Value!);
            _history?.Add(parsed.Value!);
            RollResultDisplay.Show(result);
            break;
        }
    }
}
