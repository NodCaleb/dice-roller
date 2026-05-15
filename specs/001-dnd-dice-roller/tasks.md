---
description: "Task list for DnD Dice Roller console application"
---

# Tasks: DnD Dice Roller Console Application

**Input**: Design documents from `specs/001-dnd-dice-roller/`

**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅

**Tests**: Unit tests are **MANDATORY** per Constitution Principle IV and FR-015. All service-layer tests MUST be written before implementation and MUST fail before the implementation exists.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel with other [P] tasks in the same phase (different files, no incomplete dependencies)
- **[Story]**: User story this task belongs to (US1–US4)
- Exact file paths are included in every task description

---

## Phase 1: Setup (Project Scaffold)

**Purpose**: Create the solution structure, project files, and package references. No business logic.

- [ ] T001 Initialize solution file and console app project: run `dotnet new sln -n DiceRoller` at repo root, then `dotnet new console -n DiceRoller -o src/DiceRoller --framework net10.0`; add project to solution via `dotnet sln add src/DiceRoller/DiceRoller.csproj`
- [ ] T002 [P] Create xUnit test project: run `dotnet new xunit -n DiceRoller.Tests -o tests/DiceRoller.Tests --framework net10.0`; add to solution via `dotnet sln add tests/DiceRoller.Tests/DiceRoller.Tests.csproj`
- [ ] T003 Add Spectre.Console NuGet package to `src/DiceRoller/DiceRoller.csproj` via `dotnet add src/DiceRoller package Spectre.Console`
- [ ] T004 [P] Add FakeItEasy and Microsoft.NET.Test.Sdk packages to `tests/DiceRoller.Tests/DiceRoller.Tests.csproj` via `dotnet add tests/DiceRoller.Tests package FakeItEasy` and `dotnet add tests/DiceRoller.Tests package Microsoft.NET.Test.Sdk`
- [ ] T005 Add project reference from test project to app project: run `dotnet add tests/DiceRoller.Tests reference src/DiceRoller/DiceRoller.csproj`
- [ ] T006 Enable nullable and implicit usings in both `src/DiceRoller/DiceRoller.csproj` and `tests/DiceRoller.Tests/DiceRoller.Tests.csproj` (`<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`); verify `dotnet build` succeeds on empty skeleton

**Checkpoint**: `dotnet build` and `dotnet test` succeed on the empty solution.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core models, the RNG abstraction, and the `IndexedSelectionPrompt<T>` UI helper that all user stories depend on. **No user story phase can begin until this phase is complete.**

- [ ] T007 [P] Create `DieType` enum in `src/DiceRoller/Models/DieType.cs` with members D4=4, D6=6, D8=8, D10=10, D12=12, D20=20, D100=100; include a static `AllValues` array for validation and prompt display
- [ ] T008 [P] Create `ParseResult<T>` record in `src/DiceRoller/Models/ParseResult.cs` with `bool IsSuccess`, `T? Value`, `string? ErrorMessage` and static factory methods `ParseResult<T>.Ok(T value)` and `ParseResult<T>.Fail(string message)`
- [ ] T009 Create `DiceExpression` record in `src/DiceRoller/Models/DiceExpression.cs` with `int Count`, `DieType Die`, `int Modifier` properties and `string ToCanonical()` method (rules: count always explicit, zero modifier omitted, lowercase `d`; e.g. `1d20`, `3d8+2`, `2d6-1`)
- [ ] T010 [P] Create `RollResult` record in `src/DiceRoller/Models/RollResult.cs` with `DiceExpression Expression`, `IReadOnlyList<int> DiceValues`, and computed `int Total` property (`DiceValues.Sum() + Expression.Modifier`)
- [ ] T011 [P] Create `IRandomSource` interface in `src/DiceRoller/Services/IRandomSource.cs` with single method `int Next(int minInclusive, int maxInclusive)`
- [ ] T012 Create `CryptoRandomSource` class in `src/DiceRoller/Services/CryptoRandomSource.cs` implementing `IRandomSource`; delegate to `RandomNumberGenerator.GetInt32(minInclusive, maxInclusive + 1)`; **never use `System.Random`**
- [ ] T013 Create `IndexedSelectionPrompt<T>` helper class in `src/DiceRoller/Ui/IndexedSelectionPrompt.cs`; render each choice as `[n] label`; accept arrow-key + Enter navigation **and** digit-index + Enter selection; reject out-of-range index with `[red]✗ Invalid choice. Enter a number between 1 and {count}.[/]` and re-prompt; expose `T Show(string title, IReadOnlyList<T> choices, Func<T, string> label)` method

**Checkpoint**: All models compile; `CryptoRandomSource` passes a quick manual check (`RandomNumberGenerator` referenced correctly, no `System.Random`).

---

## Phase 3: User Story 1 — Guided Roll (Priority: P1) 🎯 MVP

**Goal**: User selects die type, count, and modifier interactively; sees individual die results and total.

**Independent Test**: Launch `dotnet run --project src/DiceRoller`, select "New Roll" from the stub menu, complete all guided prompts, verify individual results and total display correctly.

### Tests — User Story 1 ⚠️ Write FIRST; verify they FAIL before implementation

- [ ] T014 Write `DiceRollerServiceTests` in `tests/DiceRoller.Tests/Rolling/DiceRollerServiceTests.cs` covering: `Roll()` returns a `RollResult` with `DiceValues.Count == expression.Count`; each value in `[1, (int)expression.Die]`; `Total == DiceValues.Sum() + Modifier`; zero modifier leaves total unchanged; positive modifier is added; negative modifier is subtracted; run `dotnet test` and confirm tests FAIL

### Implementation — User Story 1

- [ ] T015 [P] Create `IDiceRoller` interface in `src/DiceRoller/Services/IDiceRoller.cs` with method `RollResult Roll(DiceExpression expression)`
- [ ] T016 [US1] Implement `DiceRollerService` in `src/DiceRoller/Services/DiceRollerService.cs`; accept `IRandomSource` via constructor; roll `expression.Count` dice each via `_random.Next(1, (int)expression.Die)`; return new `RollResult`; run `DiceRollerServiceTests` and confirm all pass
- [ ] T017 [P] [US1] Create `RollResultDisplay` in `src/DiceRoller/Ui/RollResultDisplay.cs`; render individual dice in a Spectre.Console `Table` (columns: Die, Result; die values in yellow); render summary `Panel` (Expression, Dice total, Modifier in cyan/red, separator line, TOTAL in bold green); omit Modifier row when modifier is 0; display `"Press any key to continue…"` and wait for keypress
- [ ] T018 [US1] Create `GuidedRollPrompt` in `src/DiceRoller/Ui/GuidedRollPrompt.cs`; use `IndexedSelectionPrompt<DieType>` for die type (displays d4…d100); use `TextPrompt<int>` validated to [1..20] for count; use `TextPrompt<int>` validated to [−20..+20] for modifier (default 0, optional); return the resulting `DiceExpression`
- [ ] T019 [US1] Update `Program.cs` with a minimal two-option stub menu ("New Roll" / "Exit") using `IndexedSelectionPrompt<string>`; wire selection to `GuidedRollPrompt` → `IDiceRoller.Roll()` → `RollResultDisplay.Show()`; instantiate `CryptoRandomSource` and `DiceRollerService` in `Program.cs`

**Checkpoint**: `dotnet run --project src/DiceRoller` → select "New Roll" → complete prompts → individual dice table and total panel appear → "Press any key" returns to stub menu.

---

## Phase 4: User Story 2 — Dice Expression Entry (Priority: P2)

**Goal**: User types a dice expression string (e.g. `3d8+2`); it is parsed, validated, rolled, and displayed; invalid expressions show a helpful error.

**Independent Test**: Launch app, select "Enter Expression", type `3d8+2` → results appear. Then type `abc` → red error shown, re-prompted. Type `d20` → treated as `1d20`.

### Tests — User Story 2 ⚠️ Write FIRST; verify they FAIL before implementation

- [ ] T020 Write `DiceExpressionParserTests` in `tests/DiceRoller.Tests/Parsing/DiceExpressionParserTests.cs` covering: `d20` → Count=1, Die=D20, Modifier=0; `1d20` → same; `3d8+2` → Count=3, Die=D8, Modifier=2; `2d6-1` → Modifier=-1; `D20` (uppercase) → success (case-insensitive); `1d20+0` → Modifier=0; unsupported sides `d7` → failure with message; count 0 → failure; count 21 → failure; modifier +21 → failure; modifier −21 → failure; `abc` → failure; `5d0` → failure; `1d20 +5` (whitespace) → failure; run `dotnet test` and confirm tests FAIL

### Implementation — User Story 2

- [ ] T021 [P] Create `IDiceExpressionParser` interface in `src/DiceRoller/Services/IDiceExpressionParser.cs` with method `ParseResult<DiceExpression> Parse(string input)`
- [ ] T022 [US2] Implement `DiceExpressionParser` in `src/DiceRoller/Services/DiceExpressionParser.cs`; use compiled `Regex(@"^(?<count>\d+)?[dD](?<sides>\d+)(?<modifier>[+-]\d+)?$", RegexOptions.Compiled)`; apply validation chain: sides in allowed set → count [1..20] → modifier [−20..+20]; return `ParseResult<DiceExpression>.Ok(...)` or `.Fail(message)`; run `DiceExpressionParserTests` and confirm all pass
- [ ] T023 [US2] Create `ExpressionEntryPrompt` in `src/DiceRoller/Ui/ExpressionEntryPrompt.cs`; use `TextPrompt<string>` to read raw input; call `IDiceExpressionParser.Parse()`; on failure display `[red]✗ Error: {message}[/]` and re-prompt; on success call `IDiceRoller.Roll()` → `RollResultDisplay.Show()`
- [ ] T024 [US2] Add "Enter Expression" to the stub menu in `Program.cs`; wire to `ExpressionEntryPrompt`; instantiate and inject `DiceExpressionParser`

**Checkpoint**: US1 and US2 both work; expressions typed directly produce correct results; invalid inputs are rejected gracefully.

---

## Phase 5: User Story 3 — Reroll From History (Priority: P3)

**Goal**: The last 7 unique canonical expressions are stored in history; user can select any to reroll with one menu pick; repeated expressions move to top without duplication.

**Independent Test**: Perform two different rolls, select "Reroll Recent", verify both expressions appear with most recent first; select one and verify fresh results; roll 8 different expressions and verify only 7 remain.

### Tests — User Story 3 ⚠️ Write FIRST; verify they FAIL before implementation

- [ ] T025 Write `RollHistoryTests` in `tests/DiceRoller.Tests/History/RollHistoryTests.cs` covering: new `RollHistory` has empty `Entries`; `Add()` inserts at index 0; second `Add()` with same canonical key removes old entry and inserts at 0 (count stays same); after 7 distinct `Add()` calls, `Entries.Count == 7`; 8th distinct `Add()` drops oldest entry keeping count at 7; adding a canonical duplicate of the 7th entry moves it to index 0 without exceeding count 7; `Entries` order reflects most-recently-used first; run `dotnet test` and confirm tests FAIL

### Implementation — User Story 3

- [ ] T026 [P] Create `IRollHistory` interface in `src/DiceRoller/Services/IRollHistory.cs` with `IReadOnlyList<DiceExpression> Entries { get; }` and `void Add(DiceExpression expression)`
- [ ] T027 [US3] Implement `RollHistory` in `src/DiceRoller/Services/RollHistory.cs`; back with `List<DiceExpression>`; `Add()` algorithm: compute `expression.ToCanonical()`; remove existing entry with same key; insert at index 0; trim to max 7; run `RollHistoryTests` and confirm all pass
- [ ] T028 [P] [US3] Update `GuidedRollPrompt` in `src/DiceRoller/Ui/GuidedRollPrompt.cs` to accept and inject `IRollHistory`; call `_history.Add(expression)` after the roll is executed
- [ ] T029 [P] [US3] Update `ExpressionEntryPrompt` in `src/DiceRoller/Ui/ExpressionEntryPrompt.cs` to accept and inject `IRollHistory`; call `_history.Add(expression)` after a successful parse and roll
- [ ] T030 [US3] Create `RerollHistoryPrompt` in `src/DiceRoller/Ui/RerollHistoryPrompt.cs`; if `IRollHistory.Entries` is empty display `[yellow]No recent rolls yet.[/]` + keypress return; otherwise show up to 7 entries via `IndexedSelectionPrompt<DiceExpression>` (display canonical string); execute selected `IDiceRoller.Roll()` → `RollResultDisplay.Show()`; the re-rolled expression is added to history via `IRollHistory.Add()` (moves to top)
- [ ] T031 [US3] Add "Reroll Recent" to the stub menu in `Program.cs`; wire to `RerollHistoryPrompt`; instantiate `RollHistory` as a single shared instance; pass it to `GuidedRollPrompt`, `ExpressionEntryPrompt`, and `RerollHistoryPrompt`

**Checkpoint**: US1–US3 all functional; history accumulates correctly; repeated rolls move to top; 8th unique roll evicts the oldest.

---

## Phase 6: User Story 4 — Main Menu Navigation and Exit (Priority: P4)

**Goal**: Replace the stub menu with the full polished main menu; all four options work; exit terminates cleanly.

**Independent Test**: Launch app, verify numbered options 1–4 appear; press `4` (or arrow to Exit + Enter); process terminates with exit code 0.

- [ ] T032 [US4] Create `MainMenu` class in `src/DiceRoller/Ui/MainMenu.cs`; render app title panel; use `IndexedSelectionPrompt<string>` with options `["🎲 New Roll", "✏️  Enter Expression", "🔁 Reroll Recent", "❌ Exit"]`; loop until Exit selected; dispatch to appropriate prompt via injected dependencies
- [ ] T033 [US4] Refactor `Program.cs` to remove the stub menu; construct the full service graph (`CryptoRandomSource` → `DiceRollerService`; `DiceExpressionParser`; `RollHistory`); inject all services into `MainMenu`; start `MainMenu.Run()` loop; `Environment.Exit(0)` on Exit selection
- [ ] T034 [P] [US4] Manual validation: launch app and navigate to all 4 options using arrow keys; launch again and navigate using index numbers (type `1`, `2`, `3`, `4`); confirm both methods work on every prompt in the app
- [ ] T035 [P] [US4] Manual validation: select "Reroll Recent" with no prior rolls and verify informational message appears and app returns to menu without error

**Checkpoint**: Full application is functional end-to-end from the final main menu with both navigation modes.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Quality gate — all tests green, no constitution violations, quickstart walkthrough clean.

- [ ] T036 Run `dotnet test --logger "console;verbosity=detailed"` and confirm 100% pass for `DiceExpressionParserTests`, `DiceRollerServiceTests`, and `RollHistoryTests`
- [ ] T037 [P] Search codebase for `System.Random` usage via `grep -r "System.Random\|new Random\|Random.Shared" src/`; remove or replace any hits (constitution Principle II violation)
- [ ] T038 [P] Search codebase for raw `Console.Write` usage via `grep -r "Console\.Write" src/`; replace any hits with `AnsiConsole` equivalents (constitution Principle I violation)
- [ ] T039 Walk through `specs/001-dnd-dice-roller/quickstart.md` validation: `dotnet build` succeeds; `dotnet run --project src/DiceRoller` opens main menu; exercise all 4 options including a guided roll, an expression entry, a reroll from history, and a clean exit

---

## Dependencies & Execution Order

### Phase Dependencies

```
Phase 1 (Setup)
    └── Phase 2 (Foundational) — blocks all user story phases
            ├── Phase 3 (US1 / P1) ← MVP
            │       └── Phase 4 (US2 / P2) — reuses RollResultDisplay from US1
            │               └── Phase 5 (US3 / P3) — depends on US1 + US2 prompts having IRollHistory injection
            │                       └── Phase 6 (US4 / P4) — full menu wrapping all prompts
            │                               └── Phase 7 (Polish)
```

### User Story Dependencies

- **US1 (P1)**: Depends only on Phase 2 Foundational — independently deliverable MVP
- **US2 (P2)**: Depends on Phase 2 + US1 `RollResultDisplay` (display reuse) — independently testable otherwise
- **US3 (P3)**: Depends on Phase 2 + US1 and US2 prompts being available for `IRollHistory` injection
- **US4 (P4)**: Depends on all prompts from US1–US3 being available; replaces the stub menu

### Within Each Phase

1. Tests written first → verify FAIL → implement → verify PASS
2. Models before services; services before UI
3. Interface before implementation
4. Shared dependencies (e.g. `IndexedSelectionPrompt`) before consumers

---

## Parallel Opportunities

### Phase 1 (Setup)
T001 and T002 can be started simultaneously on separate terminals.
T003 and T004 can run in parallel after solution files exist.

### Phase 2 (Foundational)
T007, T008, T010, T011 can all be written in parallel (independent files).
T009 depends on T007 (DieType). T012 depends on T011 (IRandomSource). T013 is independent.

### Phase 3 (US1)
T015 (interface) and T017 (display) can be written in parallel with T014 (tests) once design is clear.
T018 and T017 can be written in parallel after T013.

### Phase 4 (US2)
T021 (interface) can be written in parallel with T020 (tests).

### Phase 5 (US3)
T026 (interface) can be written in parallel with T025 (tests).
T028 and T029 can be written in parallel after T027 passes.

---

## Implementation Strategy

### MVP Scope (Phase 1 + Phase 2 + Phase 3)

Completing through **Phase 3** delivers a fully functional, independently testable MVP:
- All core models and services in place
- Rolling logic tested with deterministic fakes
- Guided roll with die type, count, modifier and rich result display
- Application can be launched and used for real dice rolling

### Incremental Value Delivery

| Phase Complete | What Users Can Do |
|----------------|-------------------|
| Phase 3 (US1) | Roll any die type with guided prompts; see individual results |
| Phase 4 (US2) | Also type `3d8+2` style expressions directly |
| Phase 5 (US3) | Also reroll any of the last 7 rolls with one selection |
| Phase 6 (US4) | Full polished menu with index navigation; clean exit |
| Phase 7 | All tests green; constitution verified; quickstart validated |
