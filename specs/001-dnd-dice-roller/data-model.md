# Data Model: DnD Dice Roller Console Application

**Branch**: `001-dnd-dice-roller` | **Date**: 2026-05-15

All types reside in the `DiceRoller` project. Models live in `src/DiceRoller/Models/`; services and their interfaces live in `src/DiceRoller/Services/`.

---

## Models

### `DieType` (enum)

**File**: `Models/DieType.cs`

Enumerates the valid D&D die faces. Each member's integer value equals the number of sides, enabling direct use in display and rolling logic.

| Member | Value | Notes |
|--------|-------|-------|
| `D4` | 4 | Four-sided die |
| `D6` | 6 | Six-sided die |
| `D8` | 8 | Eight-sided die |
| `D10` | 10 | Ten-sided die |
| `D12` | 12 | Twelve-sided die |
| `D20` | 20 | Twenty-sided die |
| `D100` | 100 | Percentile die |

**Validation rule**: Only these 7 values are accepted during parsing. Any other `sides` value triggers a parse failure.

---

### `DiceExpression` (record)

**File**: `Models/DiceExpression.cs`

Represents a fully parsed and validated roll specification. This is the canonical unit stored in roll history.

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| `Count` | `int` | 1..20 | Number of dice to roll |
| `Die` | `DieType` | enum members only | The type (sides) of each die |
| `Modifier` | `int` | −20..+20 | Flat integer added to the sum of dice; 0 = no modifier |

**Key method**:
- `ToCanonical() → string` — returns the normalized string representation:
  - Format: `{Count}d{(int)Die}` or `{Count}d{(int)Die}{Modifier:+0;-0}`
  - Zero modifier is omitted: `1d20+0` → `"1d20"`
  - Count is always present: `d20` is stored as `Count=1` → `"1d20"`
  - Examples: `"1d20"`, `"3d8+2"`, `"2d6-1"`, `"1d100"`

**Equality**: Record structural equality on `(Count, Die, Modifier)` — equivalent to comparing `ToCanonical()` strings given the fixed set of valid inputs.

---

### `ParseResult<T>` (record)

**File**: `Models/ParseResult.cs`

A discriminated result type returned by `IDiceExpressionParser.Parse`. Avoids exceptions for expected validation failures.

| Property | Type | Description |
|----------|------|-------------|
| `IsSuccess` | `bool` | `true` if parsing succeeded |
| `Value` | `T?` | Populated when `IsSuccess` is `true` |
| `ErrorMessage` | `string?` | Populated when `IsSuccess` is `false`; human-readable |

**Static factory methods**:
- `ParseResult<T>.Ok(T value)` — success
- `ParseResult<T>.Fail(string message)` — failure

---

### `RollResult` (record)

**File**: `Models/RollResult.cs`

The outcome of executing a `DiceExpression`. Produced by `IDiceRoller.Roll`.

| Property | Type | Description |
|----------|------|-------------|
| `Expression` | `DiceExpression` | The expression that was rolled |
| `DiceValues` | `IReadOnlyList<int>` | Individual die results; length equals `Expression.Count`; each value in `[1, (int)Expression.Die]` |
| `Total` | `int` | Computed: `DiceValues.Sum() + Expression.Modifier` |

**Derived value**:
- `Total` — computed property, not stored field; always `sum(DiceValues) + Expression.Modifier`

**State transitions**: None — immutable record.

---

## Services & Interfaces

### `IRandomSource` / `CryptoRandomSource`

**Files**: `Services/IRandomSource.cs`, `Services/CryptoRandomSource.cs`

**Contract**:
```
IRandomSource.Next(int minInclusive, int maxInclusive) → int
```
Returns a uniformly distributed integer in `[minInclusive, maxInclusive]`.

`CryptoRandomSource` delegates to `RandomNumberGenerator.GetInt32(minInclusive, maxInclusive + 1)`.

Tests inject a `FakeItEasy` fake of `IRandomSource`.

---

### `IDiceExpressionParser` / `DiceExpressionParser`

**Files**: `Services/IDiceExpressionParser.cs`, `Services/DiceExpressionParser.cs`

**Contract**:
```
IDiceExpressionParser.Parse(string input) → ParseResult<DiceExpression>
```

**Validation rules applied** (in order):
1. Regex match `^(?<count>\d+)?[dD](?<sides>\d+)(?<modifier>[+-]\d+)?$`; reject if no match.
2. `sides` must be in `{4, 6, 8, 10, 12, 20, 100}`; reject otherwise.
3. `count` (default 1) must be in `[1, 20]`; reject otherwise.
4. `modifier` (default 0) must be in `[-20, 20]`; reject otherwise.

Error messages are human-readable and specific to the failure reason.

**Also exposes**: `DiceExpression FromParts(int count, DieType die, int modifier)` — a factory used by the guided roll prompt to build a `DiceExpression` from validated individual inputs (no string parsing needed for the guided flow).

---

### `IDiceRoller` / `DiceRollerService`

**Files**: `Services/IDiceRoller.cs`, `Services/DiceRollerService.cs`

**Contract**:
```
IDiceRoller.Roll(DiceExpression expression) → RollResult
```

Rolls `expression.Count` dice, each in `[1, (int)expression.Die]`, using the injected `IRandomSource`. Returns a `RollResult` with the individual values and computed total.

**Dependency**: `IRandomSource` (constructor injection).

---

### `IRollHistory` / `RollHistory`

**Files**: `Services/IRollHistory.cs`, `Services/RollHistory.cs`

**Contract**:
```
IRollHistory.Entries → IReadOnlyList<DiceExpression>    // index 0 = most recent
IRollHistory.Add(DiceExpression expression) → void
```

**`Add` semantics**:
1. Compute canonical key `expression.ToCanonical()`.
2. Remove any existing entry with the same canonical key.
3. Insert `expression` at index 0.
4. If `Entries.Count > 7`, remove the last item.

**Capacity**: Hard limit of 7 entries.

---

## Relationships

```
DiceExpression ──────────────> DieType (enum)
      │
      │  rolled by
      ▼
IDiceRoller.Roll() ──────────> RollResult
      │                              │
      │  uses                        │ contains
      ▼                              ▼
IRandomSource             IReadOnlyList<int> (dice values)

DiceExpression ──────────────> IRollHistory.Add()
                                     │
                                     ▼
                              IReadOnlyList<DiceExpression>
                              (ordered, max 7, deduplicated)

string input ───────────────> IDiceExpressionParser.Parse()
                                     │
                                     ▼
                              ParseResult<DiceExpression>
```

---

## Validation Summary

| Input | Rule | Error Message |
|-------|------|---------------|
| Expression string | Must match regex `^(\d+)?[dD](\d+)([+-]\d+)?$` | "Invalid dice expression. Expected format: 1d20, 3d8+2, d6-1" |
| Die sides | Must be in {4, 6, 8, 10, 12, 20, 100} | "Unsupported die type 'd{sides}'. Valid types: d4, d6, d8, d10, d12, d20, d100" |
| Count | Must be in [1, 20] | "Dice count must be between 1 and 20" |
| Modifier | Must be in [-20, 20] | "Modifier must be between -20 and +20" |
