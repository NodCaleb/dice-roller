# Quickstart: DnD Dice Roller

**Branch**: `001-dnd-dice-roller` | **Date**: 2026-05-15

## Prerequisites

| Requirement | Version |
|-------------|---------|
| .NET SDK | 10.0 or later |
| Terminal | Any (Windows Terminal, macOS Terminal, Linux shell) |

Verify your SDK version:
```bash
dotnet --version
# Expected output starts with: 10.
```

---

## Repository Layout

```text
DiceRoller.sln
src/
└── DiceRoller/          ← Console application
tests/
└── DiceRoller.Tests/    ← xUnit test project
specs/
└── 001-dnd-dice-roller/ ← Design artifacts (this file)
```

---

## Build

From the repository root:

```bash
dotnet build
```

Expected output: `Build succeeded.`

---

## Run

```bash
dotnet run --project src/DiceRoller
```

The application opens with the main menu:

```
┌─────────────────────────────┐
│  🎲 D&D Dice Roller         │
└─────────────────────────────┘

> [1] 🎲 New Roll
  [2] ✏️  Enter Expression
  [3] 🔁 Reroll Recent
  [4] ❌ Exit
```

Navigate with **arrow keys + Enter**, or type the **option number + Enter** (e.g., press `1` then Enter to start a new roll).

---

## Run Tests

```bash
dotnet test
```

All tests in `DiceRoller.Tests` will run. Expected output: `Passed!`

To run with verbose output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## Quick Roll Examples

Once the app is running, select **✏️ Enter Expression** and type any of the following:

| Expression | Meaning |
|------------|---------|
| `d20` | Roll one d20 |
| `1d20+5` | Roll one d20, add 5 (attack roll) |
| `3d8+2` | Roll three d8s, add 2 (damage roll) |
| `2d6-1` | Roll two d6s, subtract 1 |
| `1d100` | Roll percentile |
| `20d4` | Roll twenty d4s (maximum count) |

---

## Rerolling

After completing at least one roll, selecting **🔁 Reroll Recent** shows the last 7 unique expressions. Select any to reroll immediately.

---

## Development Notes

- All random number generation uses `System.Security.Cryptography.RandomNumberGenerator` — **never** `System.Random`.
- Business logic lives exclusively in `src/DiceRoller/Services/`. The `Ui/` layer contains no logic.
- See [data-model.md](data-model.md) for entity definitions and service contracts.
- See [research.md](research.md) for technology decisions and rationale.
- See [contracts/dice-expression-grammar.md](contracts/dice-expression-grammar.md) for the full dice expression grammar and CLI interaction spec.
