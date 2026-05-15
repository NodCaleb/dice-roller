# Implementation Plan: DnD Dice Roller Console Application

**Branch**: `001-dnd-dice-roller` | **Date**: 2026-05-15 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `specs/001-dnd-dice-roller/spec.md`

## Summary

Build a .NET 10 console application that lets users roll standard D&D dice interactively. Users can roll via a guided prompt, type dice expressions directly (e.g. `3d8+2`), and reroll any of the last 7 unique rolls from history. All UI uses Spectre.Console; all randomness uses `RandomNumberGenerator.GetInt32`; all business logic lives in interface-backed services tested with xUnit + FakeItEasy.

## Technical Context

**Language/Version**: C# 13 / .NET 10

**Primary Dependencies**: Spectre.Console (latest stable), xUnit 2.x, FakeItEasy (latest stable)

**Storage**: None — in-memory session only (`RollHistory` held in a list for the lifetime of the process)

**Testing**: xUnit (MUST); FakeItEasy for mocks (MUST); `dotnet test` runner

**Target Platform**: Windows / macOS / Linux terminal (local machine; no containerization)

**Project Type**: Console application (`dotnet new console`)

**Performance Goals**: Imperceptible latency for all dice rolls and parsing; console rendering must complete within a single frame

**Constraints**: No `System.Random`; no file I/O; no network; modifier range −20..+20; dice count 1..20; dice types d4/d6/d8/d10/d12/d20/d100 only

**Scale/Scope**: Single-user interactive CLI, two .NET projects (app + tests), < 20 source files

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| # | Principle | Gate Question | Status |
|---|-----------|---------------|--------|
| I | Console-First UI | Does ALL user-facing output use `Spectre.Console`? No raw `Console.WriteLine` for UI? | ✅ |
| II | Cryptographic Randomness (NON-NEGOTIABLE) | Does ALL random generation use `RandomNumberGenerator`? Zero uses of `System.Random`? | ✅ |
| III | Service-Oriented Business Logic (NON-NEGOTIABLE) | Is ALL business logic in interface-backed service classes? Zero logic in the CLI layer? | ✅ |
| IV | Unit Tests for Core Features (NON-NEGOTIABLE) | Do all service methods have unit tests? Do tests fail before implementation? | ✅ |
| V | Simplicity / No Persistence | Is there NO persistence (no DB, no files)? Is YAGNI respected (no unused abstractions)? | ✅ |

**Pre-design gate result**: PASS — all five principles are satisfied by the planned architecture.

**Post-design re-check** (after Phase 1): See end of this file.

> **Violations of NON-NEGOTIABLE principles (II, III, IV) MUST be resolved before closing any feature task.**

## Project Structure

### Documentation (this feature)

```text
specs/001-dnd-dice-roller/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
│   └── dice-expression-grammar.md
└── tasks.md             # Phase 2 output (/speckit.tasks command)
```

### Source Code (repository root)

```text
DiceRoller.sln

src/
└── DiceRoller/
    ├── DiceRoller.csproj
    ├── Program.cs
    ├── Models/
    │   ├── DieType.cs
    │   ├── DiceExpression.cs
    │   └── RollResult.cs
    ├── Services/
    │   ├── IRandomSource.cs
    │   ├── CryptoRandomSource.cs
    │   ├── IDiceExpressionParser.cs
    │   ├── DiceExpressionParser.cs
    │   ├── IDiceRoller.cs
    │   ├── DiceRollerService.cs
    │   ├── IRollHistory.cs
    │   └── RollHistory.cs
    └── Ui/
        ├── MainMenu.cs
        ├── GuidedRollPrompt.cs
        ├── ExpressionEntryPrompt.cs
        └── RollResultDisplay.cs

tests/
└── DiceRoller.Tests/
    ├── DiceRoller.Tests.csproj
    ├── Parsing/
    │   └── DiceExpressionParserTests.cs
    ├── Rolling/
    │   └── DiceRollerServiceTests.cs
    └── History/
        └── RollHistoryTests.cs
```

**Structure Decision**: Single-solution, two-project layout. `DiceRoller` contains models, services, and UI subfolders; `DiceRoller.Tests` references `DiceRoller` and exercises all service-layer logic in isolation via FakeItEasy-mocked `IRandomSource`. The UI layer is not unit-tested (Spectre.Console rendering is validated manually).

## Complexity Tracking

No principle violations. No entries required.

## Post-Design Constitution Re-Check

| # | Principle | Design Outcome | Status |
|---|-----------|----------------|--------|
| I | Console-First UI | All UI classes in `Ui/` use `AnsiConsole`; services return plain data types | ✅ |
| II | Cryptographic Randomness | `CryptoRandomSource` wraps `RandomNumberGenerator.GetInt32`; `IRandomSource` abstraction injected everywhere | ✅ |
| III | Service-Oriented Business Logic | `DiceExpressionParser`, `DiceRollerService`, `RollHistory` are interface-backed; `Ui/` classes hold no logic | ✅ |
| IV | Unit Tests | `DiceExpressionParserTests`, `DiceRollerServiceTests`, `RollHistoryTests` cover all FRs with deterministic fakes | ✅ |
| V | Simplicity | Two projects, < 20 files, no persistence, no unused layers | ✅ |

**Post-design gate result**: PASS
