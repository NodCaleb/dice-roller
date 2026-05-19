using DiceRoller.Services;
using Spectre.Console;

namespace DiceRoller.Ui;

public sealed class RerollHistoryPrompt
{
    private readonly IRollHistory _history;
    private readonly IDiceRoller _roller;

    public RerollHistoryPrompt(IRollHistory history, IDiceRoller roller)
    {
        _history = history;
        _roller = roller;
    }

    public void Run()
    {
        if (_history.Entries.Count == 0)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[yellow]No recent rolls yet.[/]");
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[grey]Press any key to continue...[/]");
            Console.ReadKey(intercept: true);
            AnsiConsole.WriteLine();
            return;
        }

        var selected = IndexedSelectionPrompt.Show(
            "Choose a roll to repeat:",
            _history.Entries.ToList(),
            e => e.ToCanonical());

        _history.Add(selected);
        var result1 = _roller.Roll(selected);
        var result2 = _roller.Roll(selected);
        RollResultDisplay.ShowDouble(result1, result2);
    }
}
