<!--
SYNC IMPACT REPORT
==================
Version change: (template) → 1.0.0 → 1.0.1 (mocking library: Moq/NSubstitute → FakeItEasy)
Modified principles: N/A — initial population from template
Added sections:
  - Core Principles (5 principles defined)
  - Technology Stack
  - Development Workflow & Quality Gates
  - Governance
Templates updated:
  - .specify/templates/plan-template.md ✅ — Constitution Check gates aligned
  - .specify/templates/tasks-template.md ✅ — Tests marked mandatory for core features
  - .specify/templates/spec-template.md ✅ — No changes required
Deferred TODOs: None
-->

# Dice Roller Constitution

## Core Principles

### I. Console-First UI (Spectre Console)

All user interaction MUST be delivered through the terminal using **Spectre Console**.
No GUI, web, or platform-native dialog is permitted.
Output MUST use Spectre Console markup, tables, or prompts — raw `Console.WriteLine` calls
for user-facing text are prohibited.
Spectre Console's `AnsiConsole` class MUST be the single entry point for all rendering.

### II. Cryptographic Randomness (NON-NEGOTIABLE)

All random number generation MUST use `System.Security.Cryptography.RandomNumberGenerator`.
`System.Random`, `Random.Shared`, or any pseudo-random alternative is **strictly forbidden**
for dice rolls or any game-outcome logic.
This ensures statistical fairness and prevents seed-based prediction attacks.

### III. Service-Oriented Business Logic (NON-NEGOTIABLE)

All game and dice logic MUST be encapsulated in dedicated service classes
(e.g., `DiceRollerService`, `RollResultService`).
The console/UI layer MUST NOT contain business logic; it delegates entirely to services.
Services MUST be interface-backed to enable injection and unit testing in isolation.
No static helper methods for business logic — all logic belongs inside a service class.

### IV. Unit Tests for Core Features (NON-NEGOTIABLE)

Unit tests MUST be authored for all service-layer methods before or alongside implementation.
Tests MUST cover: normal rolls, boundary inputs (zero dice, max dice), and invalid input handling.
Test project MUST reside in a dedicated `*.Tests` project within the same solution.
All tests MUST pass before any feature is considered complete.
`xUnit` is the MUST-use test framework; `FakeItEasy` is the MUST-use mocking library.

### V. Simplicity — No Persistence, Local Execution Only

This application runs on a local machine with no network access required.
There is **no data persistence**: no database, no files written between sessions, no cloud sync.
YAGNI strictly applies — do not add features, abstractions, or infrastructure not required
by an existing user story.
Complexity MUST be justified; every added class or layer requires a documented rationale.

## Technology Stack

- **Runtime**: .NET 10 (MUST; no downgrade)
- **UI library**: Spectre.Console (latest stable; MUST)
- **Random source**: `System.Security.Cryptography.RandomNumberGenerator` (MUST)
- **Test framework**: xUnit (MUST); FakeItEasy for mocks (MUST)
- **Language**: C# 13 (latest with .NET 10)
- **Project type**: Console application (`dotnet new console`)
- **Persistence**: None — in-memory only, no SQLite, no file I/O between sessions
- **Target platforms**: Windows, macOS, Linux (local machine; no containerization required)

## Development Workflow & Quality Gates

1. **Principle compliance check** MUST be performed before implementation starts and
   re-checked after design is complete (see Constitution Check in plan.md).
2. **Service interfaces** MUST be defined before implementation tasks begin.
3. **Tests written first** for each core service method; tests MUST fail before implementation.
4. **No PR/merge** is accepted with failing tests or untested service methods.
5. **Spectre Console** rendering code lives only in the presentation/CLI layer — never in services.
6. **No `System.Random`** usage anywhere in the codebase; CI MUST flag violations.

## Governance

This constitution supersedes all other practices and informal agreements for the Dice Roller project.
Amendments MUST increment the version following semantic versioning:
- **MAJOR**: Removal or redefinition of a non-negotiable principle.
- **MINOR**: Addition of a new principle or materially expanded guidance.
- **PATCH**: Clarification, wording fix, or non-semantic refinement.

All amendments MUST update `LAST_AMENDED_DATE` and record a Sync Impact Report
in the HTML comment at the top of this file.
Violations of NON-NEGOTIABLE principles (II, III, IV) MUST be corrected before
any feature task is closed.

**Version**: 1.0.1 | **Ratified**: 2026-05-15 | **Last Amended**: 2026-05-15
