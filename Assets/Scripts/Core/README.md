# Trigon `Core` Assembly

This assembly contains the absolute foundation of the Trigon mathematical and business logic. It possesses the most sacred architectural rule in the codebase: **Absolute purity from the game engine.**

## Architectural Rules

1. **No Unity Dependencies**: It is strictly forbidden to use `using UnityEngine;` anywhere in this assembly.
2. **Abstract Math over Unity Math**: Use native C# structs and custom definitions (like `Position2D` and `GridCoord`) instead of Unity's `Vector2` or `Mathf`. 
3. **Pure State Operations**: This assembly defines what happens internally (mathematical state manipulation, score accumulation, hex grid evaluations) when actions occur, but it does NOT interact with input handling, UI sprites, or visual representations directly.
4. **Abstract Interface Ports**: `Core` declares Interfaces (like `IDataService`, `IBoardLogic`) that the rest of the app relies on. If an interface *must* reference a Unity Type (like `Transform`), it belongs in `Modules/Board`, NOT in `Core`.

## Contents
- **`Data/`**: Raw state schemas (`BoardData`, `GameSessionData`).
- **`Logic/`**: Pure rule evaluations (`BoardLogic`, `ScoreService`).
- **`StateMachine/`** & **`States/`**: Theoretical discrete game states disconnected from visual transitions.
- **`Types/`**: Primitives like `GridCoord` (hexagonal logic axis) and `Position2D` (float pairs).
