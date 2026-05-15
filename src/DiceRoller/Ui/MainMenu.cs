using DiceRoller.Services;
using Spectre.Console;

namespace DiceRoller.Ui;

public sealed class MainMenu
{
    private readonly GuidedRollPrompt _guidedPrompt;
    private readonly ExpressionEntryPrompt _expressionPrompt;
    private readonly RerollHistoryPrompt _rerollPrompt;

    private static readonly IReadOnlyList<string> Options =
    [
        "🎲 New Roll",
        "✏️  Enter Expression",
        "🔁 Reroll Recent",
        "❌ Exit",
    ];

    public MainMenu(
        GuidedRollPrompt guidedPrompt,
        ExpressionEntryPrompt expressionPrompt,
        RerollHistoryPrompt rerollPrompt)
    {
        _guidedPrompt = guidedPrompt;
        _expressionPrompt = expressionPrompt;
        _rerollPrompt = rerollPrompt;
    }

    public void Run()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new Panel("[bold yellow]🎲 D&D Dice Roller[/]")
                    .RoundedBorder()
                    .BorderStyle(new Style(Color.Yellow)));

            var choice = IndexedSelectionPrompt.Show(
                "What would you like to do?",
                Options,
                x => x);

            if (choice == "❌ Exit")
            {
                AnsiConsole.MarkupLine("[grey]Farewell, adventurer![/]");
                break;
            }

            if (choice == "🎲 New Roll") _guidedPrompt.Run();
            else if (choice == "✏️  Enter Expression") _expressionPrompt.Run();
            else if (choice == "🔁 Reroll Recent") _rerollPrompt.Run();
        }
    }
}
