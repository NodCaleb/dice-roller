using DiceRoller.Models;
using DiceRoller.Services;
using Spectre.Console;

namespace DiceRoller.Ui;

public sealed class GuidedRollPrompt
{
    private readonly IDiceRoller _roller;
    private readonly IRollHistory? _history;

    public GuidedRollPrompt(IDiceRoller roller, IRollHistory? history = null)
    {
        _roller = roller;
        _history = history;
    }

    public void Run()
    {
        // Step 1: die type
        var dieType = IndexedSelectionPrompt.Show(
            "Choose a die type:",
            DieTypeExtensions.AllValues,
            d => d.ToLabel());

        // Step 2: number of dice
        var count = AnsiConsole.Prompt(
            new TextPrompt<int>("How many dice? [grey](1–20)[/]")
                .Validate(n => n is >= 1 and <= 20
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Enter a number between 1 and 20.[/]")));

        // Step 3: modifier
        var modifier = AnsiConsole.Prompt(
            new TextPrompt<int>("Modifier? [grey](−20 to +20, default 0)[/]")
                .DefaultValue(0)
                .Validate(m => m is >= -20 and <= 20
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Modifier must be between −20 and +20.[/]")));

        var expression = new DiceExpression(count, dieType, modifier);
        var result = _roller.Roll(expression);
        _history?.Add(expression);
        RollResultDisplay.Show(result);
    }
}
