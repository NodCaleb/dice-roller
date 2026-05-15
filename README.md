# 🎲 D&D Dice Roller

A polished .NET 10 console application for rolling Dungeons & Dragons dice, built with [Spectre.Console](https://spectreconsole.net/).

## Features

- **Guided roll** — choose die type, number of dice, and modifier step by step
- **Dice expression entry** — type expressions directly: `3d8+2`, `1d20+5`, `d20`, `2d6-1`
- **Roll history** — automatically tracks the last 7 unique rolls; reroll any with a single keypress
- **Rich output** — individual die results in a table, summary panel with color-coded modifier and bold total
- **Cryptographically fair** — all rolls use `RandomNumberGenerator` (no `System.Random`)
- Supports all standard D&D dice: **d4, d6, d8, d10, d12, d20, d100**

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later

```bash
dotnet --version   # should start with 10.
```

## Getting Started

```bash
git clone <repo-url>
cd dice-roller
dotnet run --project src/DiceRoller
```

Press a number key to select a menu option — no Enter required.

## Usage

```
╭─────────────────────╮
│  🎲 D&D Dice Roller │
╰─────────────────────╯

What would you like to do?

  [1] 🎲 New Roll
  [2] ✏️  Enter Expression
  [3] 🔁 Reroll Recent
  [4] ❌ Exit
```

### New Roll

Select a die type, number of dice (1–20), and an optional modifier (−20 to +20) using guided prompts.

### Enter Expression

Type a dice expression in standard notation:

| Expression | Meaning |
|------------|---------|
| `d20` | Roll one d20 |
| `1d20+5` | Roll one d20, add 5 |
| `3d8+2` | Roll three d8s, add 2 |
| `2d6-1` | Roll two d6s, subtract 1 |
| `1d100` | Percentile roll |

Invalid expressions are rejected with a descriptive error message.

### Reroll Recent

Displays the last 7 unique roll expressions (most recent first). Press the corresponding number to instantly reroll. Repeated rolls move to the top of the list without creating duplicates.

## Running Tests

```bash
dotnet test
```

41 unit tests cover dice expression parsing, roll calculation, modifier handling, invalid input, and history management.

```bash
dotnet test --logger "console;verbosity=detailed"
```

## Project Structure

```
DiceRoller.sln
src/
└── DiceRoller/
    ├── Models/          # DiceExpression, RollResult, DieType, ParseResult
    ├── Services/        # DiceExpressionParser, DiceRollerService, RollHistory + interfaces
    └── Ui/              # MainMenu, prompts, RollResultDisplay
tests/
└── DiceRoller.Tests/
    ├── Parsing/         # DiceExpressionParserTests
    ├── Rolling/         # DiceRollerServiceTests
    └── History/         # RollHistoryTests
```

## License

[MIT](LICENSE)
