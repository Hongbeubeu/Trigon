# Core (`Trigon.Core`)

Pure C# game engine with **no Unity engine references**. This assembly can be compiled and tested as a standalone .NET library.

## Structure

```
Core/
├── Constants/      Enums shared across the project
│   └── TypeTile    UP / DOWN tile orientation
├── Types/          Value types replacing Unity math types
│   ├── GridCoord   3-axis board coordinate (replaces Vector3Int)
│   └── Position2D  2D world position (replaces Vector2)
├── Data/           Game state models
│   ├── TileCellData      Single board cell (coord, position, type, occupancy)
│   ├── BoardData          Board grid: cell registry, axis-line mappings
│   ├── GameSessionData    Runtime session: state, score, spawn count
│   └── DataService        Owns BoardData + GameSessionData, provides reset
├── Logic/          Game rules (operate on Data, no side effects beyond events)
│   ├── BoardLogic         Position queries, placement, fit validation, line detection
│   └── ScoreService       Score tracking, max score via IScorePersistence
├── Interfaces/     Abstractions for platform-specific implementations
│   └── IScorePersistence  Load/save max score (implemented by Unity bridge)
├── Events/         Event bus for cross-layer communication
│   └── GameEvents         Static events: score, state changes, UI requests
├── StateMachine/   Generic reusable state machine
│   ├── IState             Enter / Update / Exit contract
│   └── StateMachine<T>    Enum-keyed state registry with transitions
└── States/         Concrete game states
    ├── GameContext         Shared context: session, score service, state machine
    ├── PlayingState        Sets Playing state, fires event
    ├── PausedState         Sets Paused state, fires event
    └── LostState           Saves max score, sets Lost state, fires event
```

## Key Principles

- **No `using UnityEngine`** — all files use `System`, `System.Collections.Generic`, `System.MathF`
- **Constructor injection** — all dependencies passed via constructors, no service locator
- **Testable** — can be unit tested with NUnit/xUnit without Unity test runner
- **Portable** — game logic works on any .NET runtime (server, console, other engines)
