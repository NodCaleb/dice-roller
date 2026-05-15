using Spectre.Console;

namespace DiceRoller.Ui;

/// <summary>
/// A selection prompt that renders 1-based numeric indices next to each choice.
/// Pressing a digit key selects the matching item immediately (no Enter required).
/// Out-of-range or non-digit keys show an error and re-render the menu.
/// </summary>
public static class IndexedSelectionPrompt
{
    public static T Show<T>(string title, IReadOnlyList<T> choices, Func<T, string> label)
        where T : notnull
    {
        while (true)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[bold]{Markup.Escape(title)}[/]");
            AnsiConsole.WriteLine();

            for (int i = 0; i < choices.Count; i++)
                AnsiConsole.MarkupLine($"  [grey][[{i + 1}]][/] {Markup.Escape(label(choices[i]))}");

            AnsiConsole.WriteLine();
            AnsiConsole.Markup($"[grey]Press a number key (1–{choices.Count}): [/]");

            var key = Console.ReadKey(intercept: true);
            AnsiConsole.WriteLine();

            if (key.KeyChar >= '1' && key.KeyChar <= '9')
            {
                int index = key.KeyChar - '0';
                if (index >= 1 && index <= choices.Count)
                    return choices[index - 1];
            }

            AnsiConsole.MarkupLine(
                $"[red]✗ Invalid choice. Press a number between 1 and {choices.Count}.[/]");
        }
    }
}
