using DiceRoller.Models;
using Spectre.Console;

namespace DiceRoller.Ui;

public static class RollResultDisplay
{
    public static void Show(RollResult result)
    {
        AnsiConsole.WriteLine();

        // Individual dice table
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Die[/]").Centered())
            .AddColumn(new TableColumn("[bold]Result[/]").Centered());

        var dieLabel = result.Expression.Die.ToLabel();
        foreach (var value in result.DiceValues)
            table.AddRow(dieLabel, $"[yellow]{value}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        // Summary panel
        var lines = new List<string>
        {
            $"[grey]Expression :[/] [white]{Markup.Escape(result.Expression.ToCanonical())}[/]",
            $"[grey]Dice total :[/] [white]{result.DiceValues.Sum()}[/]",
        };

        if (result.Expression.Modifier != 0)
        {
            var modColor = result.Expression.Modifier > 0 ? "cyan" : "red";
            var modLabel = result.Expression.Modifier > 0
                ? $"+{result.Expression.Modifier}"
                : $"{result.Expression.Modifier}";
            lines.Add($"[grey]Modifier   :[/] [{modColor}]{modLabel}[/]");
        }

        lines.Add("[grey]─────────────────────────────[/]");
        lines.Add($"[grey]TOTAL      :[/] [bold green]{result.Total}[/]");

        var panel = new Panel(string.Join("\n", lines))
        {
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Yellow),
        };

        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
        AnsiConsole.Markup("[grey]Press any key to continue...[/]");
        Console.ReadKey(intercept: true);
        AnsiConsole.WriteLine();
    }
}
