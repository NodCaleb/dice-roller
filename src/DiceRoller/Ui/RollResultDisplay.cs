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

    public static void ShowDouble(RollResult first, RollResult second)
    {
        AnsiConsole.WriteLine();

        AnsiConsole.MarkupLine($"[grey]Expression :[/] [white]{Markup.Escape(first.Expression.ToCanonical())}[/]");
        AnsiConsole.WriteLine();

        // Combined dice table: Die | Roll 1 | Roll 2
        var table = new Table()
            .RoundedBorder()
            .BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Die[/]").Centered())
            .AddColumn(new TableColumn("[bold]Roll 1[/]").Centered())
            .AddColumn(new TableColumn("[bold]Roll 2[/]").Centered());

        var dieLabel = first.Expression.Die.ToLabel();
        var maxRows = Math.Max(first.DiceValues.Count, second.DiceValues.Count);
        for (var i = 0; i < maxRows; i++)
        {
            var v1 = i < first.DiceValues.Count ? $"[yellow]{first.DiceValues[i]}[/]" : "";
            var v2 = i < second.DiceValues.Count ? $"[yellow]{second.DiceValues[i]}[/]" : "";
            table.AddRow(dieLabel, v1, v2);
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        // Two summary panels side by side
        AnsiConsole.Write(new Columns(
            BuildSummaryPanel(first, "Roll 1"),
            BuildSummaryPanel(second, "Roll 2")));
        AnsiConsole.WriteLine();

        AnsiConsole.Markup("[grey]Press any key to continue...[/]");
        Console.ReadKey(intercept: true);
        AnsiConsole.WriteLine();
    }

    private static Panel BuildSummaryPanel(RollResult result, string title)
    {
        var lines = new List<string>
        {
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

        lines.Add("[grey]─────────────────[/]");
        lines.Add($"[grey]TOTAL      :[/] [bold green]{result.Total}[/]");

        return new Panel(string.Join("\n", lines))
        {
            Header = new PanelHeader($" [bold]{title}[/] "),
            Border = BoxBorder.Rounded,
            BorderStyle = new Style(Color.Yellow),
        };
    }
}
