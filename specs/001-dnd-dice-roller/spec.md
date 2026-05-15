# Feature Specification: DnD Dice Roller Console Application

**Feature Branch**: `001-dnd-dice-roller`

**Created**: 2026-05-15

**Status**: Draft

**Input**: User description: "Create a small .NET console application for rolling Dungeons & Dragons dice using Spectre.Console for the console user interface."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Roll Dice With Guided Prompts (Priority: P1)

A player opens the application and selects "New Roll" from the main menu. The application presents a series of prompts asking for the dice type (d4, d6, d8, d10, d12, d20, d100), the number of dice to roll, and an optional numeric modifier. After confirming, the application displays each individual die result alongside the modifier and the final total.

**Why this priority**: This is the core feature of the application. Without the ability to roll dice interactively, nothing else has value. It also validates that rolling logic, individual result display, and modifier handling all work correctly.

**Independent Test**: Can be fully tested by launching the app, selecting "New Roll", completing all prompts, and verifying that results are shown — delivers a working dice roller with no other features active.

**Acceptance Scenarios**:

1. **Given** the application is running and showing the main menu, **When** the user selects "New Roll", **Then** the application prompts user to choose a dice type, number of dice, and an optional modifier in sequence.
2. **Given** the user has selected 3d8 with a +2 modifier, **When** the roll is executed, **Then** the application displays three individual d8 results (each between 1 and 8), the +2 modifier, and the final total (sum of dice + 2).
3. **Given** the user has selected 1d20 with no modifier, **When** the roll is executed, **Then** the application displays one result between 1 and 20, no modifier line, and the final total equals the single die result.
4. **Given** the user is prompted for a modifier, **When** the user enters 0 or leaves it blank, **Then** no modifier is shown in the result output.

---

### User Story 2 - Roll Dice Using a Dice Expression (Priority: P2)

A player selects "Enter Expression" from the main menu and types a dice expression such as `3d8+2`, `1d20+5`, `2d6-1`, or `d20`. The application parses the dice expression, validates it, rolls the dice, and displays the results. If the dice expression is invalid, a clear error message is shown and the user can try again.

**Why this priority**: Dice expressions are the natural language of tabletop gaming. Enabling fast expression entry makes the app useful for experienced players without extra prompts, but it depends on the roll display logic established in P1.

**Independent Test**: Can be fully tested by launching the app, selecting "Enter Expression", submitting a valid dice expression, and verifying results — and separately submitting an invalid string to confirm an error appears.

**Acceptance Scenarios**:

1. **Given** the user selects "Enter Notation", **When** the user types `3d8+2`, **Then** the application rolls 3 eight-sided dice, adds 2, and displays individual results and the total.
2. **Given** the user selects "Enter Notation", **When** the user types `d20`, **Then** the application treats it as `1d20`, rolls one die, and displays the result.
3. **Given** the user selects "Enter Notation", **When** the user types `2d6-1`, **Then** the application rolls 2 six-sided dice, subtracts 1, and displays individual results and the final total.
4. **Given** the user selects "Enter Notation", **When** the user types an invalid string such as `abc` or `5d0`, **Then** the application displays a clear, user-friendly error message and does not crash.
5. **Given** a valid dice expression is entered, **When** that dice expression is already in recent history, **Then** the expression moves to the top of the history rather than creating a duplicate.

---

### User Story 3 - Reroll From History (Priority: P3)

After rolling several different expressions, a player selects "Reroll Recent" from the main menu. The application displays the last up to 7 unique dice expressions. The player selects one and the application immediately re-executes that roll, displaying new results. The selected expression moves to the top of the history list.

**Why this priority**: Rerolling is a high-frequency action in D&D (attack rolls, damage rolls repeated across turns). This feature delivers significant time savings, but it requires the rolling and history features from P1 and P2 to be in place first.

**Independent Test**: Can be fully tested by performing at least two different rolls, then selecting "Reroll Recent", choosing a previous roll, and verifying the result is fresh and the history order updates.

**Acceptance Scenarios**:

1. **Given** the user has rolled three different expressions, **When** the user selects "Reroll Recent", **Then** a menu shows those three expressions and the user can select one to reroll.
2. **Given** the user selects an expression from recent history, **When** the reroll executes, **Then** new dice results are generated and displayed, and the expression moves to the top of the history list.
3. **Given** the history contains 7 unique expressions and the user rolls a new unique expression, **When** the history is updated, **Then** the oldest expression is removed and the new one is added at the top.
4. **Given** the user rolls an expression that already exists in history, **When** the history is updated, **Then** the existing entry moves to the top without creating a duplicate.
5. **Given** no rolls have been performed yet, **When** the user selects "Reroll Recent", **Then** the application informs the user that no recent rolls are available.

---

### User Story 4 - Navigate the Main Menu and Exit (Priority: P4)

A player opens the application and is presented with a clear main menu. They can navigate to any feature or choose to exit. The application exits cleanly when the exit option is selected.

**Why this priority**: The main menu is the entry point to all features and must work reliably, but all other stories are more valuable than the navigation shell itself.

**Independent Test**: Can be fully tested by launching the application, verifying the main menu appears with all expected options, and selecting "Exit" to confirm the application terminates cleanly.

**Acceptance Scenarios**:

1. **Given** the application starts, **When** the main menu is displayed, **Then** it shows numbered options (1–4) for: New Roll, Enter Expression, Reroll Recent, and Exit — and the user can select by arrow keys + Enter **or** by typing the option index + Enter.
2. **Given** the main menu is shown, **When** the user selects "Exit" (by arrow or by typing `4`), **Then** the application terminates without errors.
3. **Given** the user completes a roll or encounters an error, **When** the result or error is shown, **Then** the application displays a "Press any key to continue…" prompt and returns to the main menu only after the user confirms.

---

### Edge Cases

- What happens when the user enters `d20` without a count prefix? → Treated as `1d20`.
- What happens when the user enters 0 or a negative number of dice in guided mode? → Rejected with a clear validation error; user is prompted again.
- What happens when the modifier causes the final total to be zero or negative? → Allowed; the total is displayed as-is.
- What happens when the history already contains 7 entries and the user rolls a new unique expression? → The oldest entry is dropped from the bottom; the new expression is added to the top.
- What happens when a user submits an expression that is identical to one already in history? → The existing entry moves to the top; no duplicate is added; no "already in history" penalty.
- What happens when the user rolls a dice type not in the standard DnD set (e.g., d7)? → Rejected with a clear validation error; only d4, d6, d8, d10, d12, d20, d100 are accepted.
- What happens when the user specifies more than 20 dice (e.g., `25d6`)? → Rejected with a clear validation error; the maximum is 20 dice per roll.
- What happens when the user specifies a modifier outside −20 to +20 (e.g., `1d20+99`)? → Rejected with a clear validation error; the modifier must be in the range −20 to +20.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The application MUST support rolling d4, d6, d8, d10, d12, d20, and d100 dice types.
- **FR-002**: The application MUST allow the user to specify the number of dice to roll (minimum 1, maximum 20); values outside this range MUST be rejected with a clear validation error.
- **FR-003**: The application MUST allow the user to specify a positive or negative integer modifier in the range −20 to +20 (inclusive), including zero / no modifier; values outside this range MUST be rejected with a clear validation error.
- **FR-004**: The application MUST display each individual die result and the final total after every roll.
- **FR-005**: The application MUST clearly display the modifier value in the result output when a non-zero modifier is applied.
- **FR-006**: The application MUST accept standard dice expressions in the format `XdY`, `XdY+Z`, `XdY-Z`, and `dY` (which MUST be treated as `1dY`).
- **FR-007**: The application MUST validate all dice expression input and display a clear, user-friendly error message for any unparseable or out-of-range dice expression; it MUST NOT crash on invalid input.
- **FR-008**: The application MUST maintain a roll history of up to 7 unique **canonical** dice expressions per session. A canonical dice expression is produced by: (1) treating a missing count as 1 (`d20` → `1d20`), (2) stripping a zero modifier (`1d20+0` → `1d20`), and (3) lowercasing the `d` separator.
- **FR-009**: Rolling the same canonical expression as one already in history MUST move that entry to the top of the history without creating a duplicate.
- **FR-010**: When a new unique expression is added to a full history (7 entries), the oldest entry MUST be removed.
- **FR-011**: The application MUST allow the user to reroll any expression in the recent history with a single menu selection.
- **FR-012**: The main menu MUST provide at least four options: guided new roll, enter expression, reroll recent, and exit.
- **FR-016**: After displaying any roll result or error message, the application MUST show a "Press any key to continue…" prompt and wait for a keypress before returning to the main menu.
- **FR-013**: The application MUST use Spectre.Console selection prompts for menu interactions, tables or panels for result display, and colored output to distinguish result components (individual dice, modifier, total). All selection menus (main menu, die-type selection, reroll history) MUST display a 1-based index next to each choice and accept both arrow-key navigation + Enter **and** typing the index number + Enter as equally valid input methods.
- **FR-014**: Dice rolling logic MUST use a random number generator abstraction to enable deterministic unit testing.
- **FR-015**: The application MUST include unit tests covering: dice expression parsing, roll total calculation, modifier handling, invalid input handling, history uniqueness enforcement, history size limit of 7, and promotion of repeated expressions to the top of history.

### Key Entities

- **DiceExpression**: Represents a parsed roll specification — number of dice (default 1), dice type (sides), and integer modifier (default 0). Stored in history in canonical form: count always explicit, zero modifier omitted, lowercase `d`. Two expressions are equal if their canonical string representations match.
- **RollResult**: The outcome of rolling a DiceExpression — the list of individual die values, the modifier, and the computed total (sum of dice values plus modifier).
- **RollHistory**: An ordered collection of the most recent unique DiceExpression values (maximum 7), where the most recently used expression is always at the top.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can complete a full guided roll (select type, count, modifier, view results) in 4 or fewer interactive selections.
- **SC-002**: All 7 standard DnD dice types (d4, d6, d8, d10, d12, d20, d100) can be rolled successfully without error.
- **SC-003**: Dice expression parsing and result display complete instantaneously (imperceptible delay) for any valid dice expression.
- **SC-004**: Roll history never contains more than 7 entries and never contains duplicate canonical dice expressions.
- **SC-005**: A repeated dice expression is always promoted to position 1 in history without increasing the history count.
- **SC-006**: Every invalid dice expression input is handled gracefully with a descriptive error message; the application never terminates unexpectedly due to bad input.
- **SC-007**: All required unit test suites (parsing, rolling, modifiers, invalid input, history uniqueness, history limit, history promotion) pass with 100% of specified test cases succeeding.

## Clarifications

### Session 2026-05-15

- Q: What normalization rules define a "unique" expression for history deduplication? → A: Normalize to canonical form on entry: `d20` → `1d20`, `1d20+0` → `1d20`, case-insensitive (`D20` = `1d20`).
- Q: How does the user return to the main menu after a roll result is displayed? → A: Display results, then show "Press any key to continue…" and wait for a keypress before returning to the menu.
- Q: What is the maximum number of dice allowed per roll? → A: Maximum 20 dice per roll; anything above is rejected with a validation error.
- Q: What is the allowed range for the numeric modifier? → A: Modifier range −20 to +20; values outside this range are rejected with a validation error.
- Q: What is the canonical terminology for a roll string like `3d8+2`? → A: **Dice expression** is the canonical term; "dice notation" and "roll expression" are avoided as synonyms.

## Assumptions

- The application targets .NET 8 (LTS) or later on Windows, macOS, and Linux terminal environments.
- Roll history is in-memory only per session; no persistence between application launches is required.
- Dice rolls use a statistically uniform random distribution (cryptographic quality is not required).
- The modifier is a single integer value only; compound modifiers such as `1d4+1d6` are out of scope.
- The application is used interactively by a human; scripting/piping/non-interactive mode is out of scope.
- Spectre.Console is available as a NuGet package dependency and is the sole UI rendering library.
- Advantage/disadvantage mechanics (roll twice, take higher/lower) are out of scope for this version.
- Character sheet management, persistent profiles, and cloud features are out of scope.
- No database or network connectivity is required or permitted.
