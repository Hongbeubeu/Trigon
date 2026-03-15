# Trigon

A triangle-based puzzle game clone built in Unity. Place composite triangle pieces on a hexagonal board to complete lines across three axes (X, Y, Z) and score points.

## Demo

![Demo](https://user-images.githubusercontent.com/44913755/132392055-6fb182c0-811e-4f04-8d58-90f20d6b8b0f.PNG)

## Features

- Triangle-based puzzle mechanics with drag-and-drop placement
- 3D coordinate system (X, Y, Z axes) for line matching
- Line-clear scoring with combo multiplier for simultaneous clears
- Pause, resume, and replay via event-driven popup system
- Persistent high score tracking

## Architecture

The project follows a **clean architecture** with pure C# core gameplay and Unity as a thin bridge layer.

```
Assets/Scripts/
├── Core/           Pure C# game engine (no Unity dependency)
├── Modules/Board/  Unity bridge: MonoBehaviours, ScriptableObjects, views
├── UI/             UI framework: HUD, popup system, popup implementations
└── Utils/          Reusable utilities: ServiceLocator pattern
```

Each layer is compiled as a separate **Assembly Definition**:

| Assembly | Engine Refs | Purpose |
|---|---|---|
| `Trigon.Core` | None | Game rules, data, logic, state machine |
| `Trigon.Board` | Unity | MonoBehaviours, visual rendering, input |
| `Trigon.UI` | Unity + TMPro | HUD, popup manager, concrete popups |
| `Trigon.Utils` | Unity | ServiceLocator IoC container |

See each folder's README for details:
- [Core](Assets/Scripts/Core/README.md) - Pure C# gameplay engine
- [Modules/Board](Assets/Scripts/Modules/Board/README.md) - Unity bridge and game components
- [UI](Assets/Scripts/UI/README.md) - UI framework and popups
- [Utils](Assets/Scripts/Utils/README.md) - Utility patterns

## Key Design Patterns

- **State Machine** - Generic `StateMachine<T>` manages game states (Playing, Paused, Lost)
- **Service Locator** - Lightweight IoC container for dependency resolution
- **Event Bus** - Static `GameEvents` decouples systems via `Action` delegates
- **ScriptableObject Databases** - `LogicConfig`, `GameViewConfig`, `ColorPalette` for data-driven design
- **Popup System** - `BasePopup`/`PopupManager` framework with self-managed popup instances

## Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/Hongbeubeu/trigon-game-clone.git
   ```
2. Open the project in Unity 2021.3+
3. Press Play to start testing

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
