# Contract: Dice Expression Grammar and CLI Interaction

**Branch**: `001-dnd-dice-roller` | **Date**: 2026-05-15

This document defines the user-facing contracts for the DnD Dice Roller application: the grammar for dice expression strings and the CLI interaction model.

---

## 1. Dice Expression Grammar

### 1.1 Formal Grammar (EBNF)

```ebnf
dice-expression   = [ count ] "d" sides [ modifier ]
count             = positive-integer          (* 1..20; if absent, defaults to 1 *)
sides             = "4" | "6" | "8" | "10" | "12" | "20" | "100"
modifier          = sign integer              (* result: -20..+20 *)
sign              = "+" | "-"
positive-integer  = digit { digit }           (* no leading zeros required *)
integer           = digit { digit }
digit             = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
```

The grammar is **case-insensitive** (`d` and `D` are equivalent). Whitespace is not permitted inside an expression.

### 1.2 Regex (Reference Implementation)

```regex
^(?<count>\d+)?[dD](?<sides>\d+)(?<modifier>[+-]\d+)?$
```

Applied with `RegexOptions.IgnoreCase | RegexOptions.Compiled`.

### 1.3 Valid Expression Examples

| Input | Canonical Form | Meaning |
|-------|----------------|---------|
| `d20` | `1d20` | Roll one d20, no modifier |
| `1d20` | `1d20` | Roll one d20, no modifier |
| `D20` | `1d20` | Same — case-insensitive |
| `1d20+5` | `1d20+5` | Roll one d20, add 5 |
| `3d8+2` | `3d8+2` | Roll three d8s, add 2 |
| `2d6-1` | `2d6-1` | Roll two d6s, subtract 1 |
| `1d20+0` | `1d20` | Zero modifier stripped from canonical form |
| `20d4` | `20d4` | Roll twenty d4s, no modifier (maximum count) |
| `1d100` | `1d100` | Roll one d100 (percentile) |

### 1.4 Invalid Expression Examples

| Input | Reason |
|-------|--------|
| `abc` | Does not match grammar |
| `5d0` | `d0` is not a valid die type |
| `5d7` | `d7` is not a valid die type |
| `21d6` | Count exceeds maximum of 20 |
| `0d6` | Count below minimum of 1 |
| `1d6+99` | Modifier exceeds maximum of +20 |
| `1d6-99` | Modifier below minimum of −20 |
| `1d20 +5` | Whitespace inside expression |
| `1d20++5` | Double sign not supported |

### 1.5 Canonical Form Rules

1. **Count**: Always emitted explicitly (never omitted). `d20` → `1d20`.
2. **Die separator**: Always lowercase `d`. `1D20` → `1d20`.
3. **Zero modifier**: Omitted entirely. `1d20+0` → `1d20`.
4. **Non-zero modifier**: Emitted with explicit sign. `3d8+2`, `2d6-1`.

---

## 2. CLI Interaction Contract

### 2.1 Main Menu

The application presents a numbered selection prompt with the following fixed options (in this order):

| Index | Option Label | Action |
|-------|---|---|
| 1 | `🎲 New Roll` | Launch guided roll prompt |
| 2 | `✏️  Enter Expression` | Launch free-text dice expression entry |
| 3 | `🔁 Reroll Recent` | Display roll history selection |
| 4 | `❌ Exit` | Terminate the application |

**Navigation**: The user may select an option by **arrow keys + Enter** or by **typing the 1-based index + Enter**. Both methods are equally valid on every selection menu in the application.

The "Reroll Recent" option is always displayed; if history is empty, selecting it shows an informational message and returns to the menu.

### 2.2 Guided Roll Flow

Sequential prompts:

1. **Die type**: Numbered selection prompt showing `[1] d4  [2] d6  [3] d8  [4] d10  [5] d12  [6] d20  [7] d100` — accepts arrow+Enter or index+Enter.
2. **Number of dice**: `TextPrompt<int>` — validated inline (1..20); re-prompts on invalid input.
3. **Modifier** (optional): `TextPrompt<int>` — default 0; validated (−20..+20); re-prompts on invalid.

After all inputs, the roll is executed and result is displayed (see §2.4).

### 2.3 Enter Expression Flow

1. `TextPrompt<string>` — user types a dice expression string.
2. If parsing fails → display error message in red; re-prompt.
3. If parsing succeeds → execute roll; display result (see §2.4).

### 2.4 Roll Result Display

After every roll, the following is rendered:

**Individual dice table** (Spectre.Console `Table`):
```
┌─────┬────────┐
│ Die │ Result │
├─────┼────────┤
│ d8  │   6    │
│ d8  │   3    │
│ d8  │   7    │
└─────┴────────┘
```

**Summary panel** (Spectre.Console `Panel`):
```
╭────────────────────────────────╮
│  Expression : 3d8+2            │
│  Dice total : 16               │
│  Modifier   : +2               │
│  ──────────────────────────    │
│  TOTAL      : 18               │
╰────────────────────────────────╯
```

The modifier line is omitted when the modifier is 0. Color scheme:
- Individual die values: yellow
- Modifier value: cyan (positive) or red (negative)
- Final total: bold green

After results are displayed:

```
Press any key to continue...
```

The application waits for a keypress, then returns to the main menu.

### 2.5 Reroll Recent Flow

1. If history is empty: display `[yellow]No recent rolls yet.[/]`, then return to menu via keypress.
2. If history is non-empty: numbered selection prompt showing up to 7 canonical expressions (most recent first), each prefixed with its 1-based index. The user may select by arrow+Enter or by typing the index+Enter.
3. User selects one → execute roll → display result as per §2.4.

### 2.6 Error Display Contract

All validation errors are shown as:
```
[red]✗ Error:[/] <specific error message>
```

Followed by re-prompting (for guided / expression entry) or returning to menu (for unrecoverable states). The application never displays a stack trace or .NET exception message to the user.

### 2.7 Exit

Selecting "Exit" from the main menu (by arrow+Enter or by typing `4`+Enter) terminates the process immediately with exit code `0`. No confirmation prompt is shown.

---

## 3. Indexed Selection Prompt Implementation Notes

Spectre.Console's built-in `SelectionPrompt<T>` does not natively accept numeric index input. The implementation MUST provide a custom helper (e.g., `IndexedSelectionPrompt<T>`) that:

1. Renders each choice with its 1-based index label (e.g., `[1] 🎲 New Roll`).
2. Supports arrow-key navigation + Enter via the standard Spectre.Console interaction loop.
3. Also accepts a digit sequence + Enter as direct index selection (e.g., typing `3` selects the third choice).
4. Rejects out-of-range index input with `[red]✗ Invalid choice. Enter a number between 1 and {count}.[/]` and re-prompts.

All four selection contexts (main menu, die-type selection, reroll history, and any future menus) MUST use this helper exclusively — not the raw `SelectionPrompt<T>`.
