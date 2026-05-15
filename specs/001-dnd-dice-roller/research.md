# Research: DnD Dice Roller Console Application

**Branch**: `001-dnd-dice-roller` | **Date**: 2026-05-15

## R-001: Cryptographic RNG for Bounded Integer Dice Rolls

**Decision**: Use `System.Security.Cryptography.RandomNumberGenerator.GetInt32(int fromInclusive, int toExclusive)`.

**Rationale**: Available since .NET 6. Produces a cryptographically strong, uniformly distributed integer in `[fromInclusive, toExclusive)` using rejection sampling internally. Eliminates the modulo-bias problem present in naive implementations. No manual rejection loop needed by the caller.

**Usage pattern**:
```csharp
// Roll 1dN: result is in [1, N] inclusive
int result = RandomNumberGenerator.GetInt32(1, sides + 1);
```

**Abstraction**: Wrap in `IRandomSource` with a single method `int Next(int minInclusive, int maxInclusive)`. The production implementation (`CryptoRandomSource`) calls `RandomNumberGenerator.GetInt32(minInclusive, maxInclusive + 1)`. Tests inject a `FakeItEasy` fake of `IRandomSource` with deterministic return values.

**Alternatives considered**:
- `System.Random` — forbidden by Constitution Principle II; statistically weaker and seed-predictable.
- `Random.Shared` — same prohibition; it is a `System.Random` instance.
- Manual rejection sampling with `RandomNumberGenerator.GetBytes` — unnecessary; `GetInt32` handles this internally.

---

## R-002: Spectre.Console API Patterns for This Application

**Decision**: Use `AnsiConsole` as the sole rendering entry point throughout the `Ui/` layer.

**Key APIs selected**:

| Purpose | API |
|---------|-----|
| Main menu selection | `AnsiConsole.Prompt(new SelectionPrompt<string>().Title(...).AddChoices(...))` |
| Guided die-type selection | `SelectionPrompt<DieType>` with `.UseConverter(dt => $"d{(int)dt}")` |
| Numeric input (count, modifier) | `AnsiConsole.Prompt(new TextPrompt<int>(...).Validate(...))` |
| Free-text dice expression entry | `AnsiConsole.Prompt(new TextPrompt<string>(...))` |
| Roll result table | `new Table()` with columns: Die, Result; rendered via `AnsiConsole.Write(table)` |
| Result summary panel | `new Panel(...)` wrapping total + modifier markup; rendered via `AnsiConsole.Write(panel)` |
| Colored inline text | `AnsiConsole.MarkupLine("[green]text[/]")`, `"[yellow]"`, `"[red]"` |
| "Press any key to continue" | `AnsiConsole.Console.Input.ReadKey(intercept: false)` |
| Error messages | `AnsiConsole.MarkupLine("[red]Error: {message}[/]")` |

**Rationale**: `AnsiConsole` is the static façade for the default console sink; it requires no DI and is the standard pattern for console apps with Spectre.Console. The static API is acceptable here because the UI layer is not unit-tested (Spectre.Console's rendering is not designed for headless testing). All testable logic is in services that have no Spectre.Console dependency.

**Alternatives considered**:
- Injecting `IAnsiConsole` — more testable but significantly increases boilerplate for a small app; YAGNI given Constitution Principle V.

---

## R-003: Dice Expression Parsing Strategy

**Decision**: Parse using a single compiled `Regex` with named capture groups; validate captured values against allowed ranges and die types.

**Canonical grammar** (see also `contracts/dice-expression-grammar.md`):
```
pattern: ^(?<count>\d+)?[dD](?<sides>\d+)(?<modifier>[+-]\d+)?$
```

**Parsing steps**:
1. Trim and apply regex (case-insensitive via `RegexOptions.IgnoreCase`).
2. If no match → return parse failure with message "Invalid dice expression format".
3. Extract `count` (default `1` if absent), `sides`, `modifier` (default `0`).
4. Validate `sides` is in `{4, 6, 8, 10, 12, 20, 100}` → reject with "Unsupported die type" if not.
5. Validate `count` in `[1, 20]` → reject with "Dice count must be between 1 and 20" if not.
6. Validate `modifier` in `[-20, 20]` → reject with "Modifier must be between −20 and +20" if not.
7. Return a valid `DiceExpression`.

**Return type**: `ParseResult<DiceExpression>` — a discriminated union with `Success(DiceExpression)` and `Failure(string errorMessage)`. Implemented as a simple record with a boolean `IsSuccess` flag and optional value/error fields (no external library needed).

**Alternatives considered**:
- Hand-rolled character-by-character parser — more code, same result, more failure surface.
- Using a parsing library (e.g., Sprache, Pidgin) — overkill for a single-rule grammar; violates YAGNI.

---

## R-004: Canonical Dice Expression Form

**Decision**: Canonical form is produced at parse time and stored in `DiceExpression.ToCanonical()`.

**Rules** (per spec clarification):
1. Always emit the count, even when 1: `d20` → `1d20`.
2. Always lowercase `d`: `1D20` → `1d20`.
3. Omit zero modifier: `1d20+0` → `1d20`.
4. Emit negative modifier with explicit sign: `2d6-1` stays `2d6-1`.

**Equality**: Two `DiceExpression` instances are equal if and only if `ToCanonical()` returns the same string. `RollHistory` deduplicates using this canonical key.

**Alternatives considered**:
- Structural equality on `(Count, Sides, Modifier)` tuple — equivalent result; canonical string chosen because it is human-readable in the history list and unambiguous.

---

## R-005: Roll History Deduplication and Ordering Algorithm

**Decision**: Maintain `RollHistory` as a `List<DiceExpression>` (max 7 items, index 0 = most recent).

**Add(expression) algorithm**:
1. Compute canonical key = `expression.ToCanonical()`.
2. Find existing index `i` where `Entries[i].ToCanonical() == canonical key`.
3. If found: remove at index `i`.
4. Insert `expression` at index 0.
5. If `Entries.Count > 7`: remove last item.

**Complexity**: O(7) in all cases — acceptable for a list bounded to 7 items.

**Alternatives considered**:
- `LinkedList<T>` — marginally faster move-to-front but adds API complexity with no measurable benefit at size 7.
- `Dictionary<string, DiceExpression>` for dedup with separate ordered list — over-engineered for 7 items.

---

## R-006: Project / Solution Layout (.NET 10)

**Decision**: Single solution file (`DiceRoller.sln`) with two projects.

| Project | SDK | Purpose |
|---------|-----|---------|
| `src/DiceRoller/DiceRoller.csproj` | `Microsoft.NET.Sdk` | Console app; `<OutputType>Exe</OutputType>` |
| `tests/DiceRoller.Tests/DiceRoller.Tests.csproj` | `Microsoft.NET.Sdk` | xUnit test project; `<ProjectReference>` to app |

**Target framework**: `net10.0` for both projects.

**NuGet packages**:
- `DiceRoller`: `Spectre.Console` (latest stable)
- `DiceRoller.Tests`: `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `FakeItEasy`

**Alternatives considered**:
- Separate `DiceRoller.Core` class library — cleaner dependency direction but unnecessary indirection for a < 20-file project; violates YAGNI (Constitution Principle V).
