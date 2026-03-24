# Trigon `Modules.Board` Assembly

This module represents the active Gameplay implementation layer. It marries the pure mathematical `Core` structures to tangible Unity rendering, input, and lifecycle events.

## Architectural Rules

1. **SOLID Principles Enforcement**: Scripts must adhere strictly to SOLID, especially Single Responsibility. God classes are not permitted; components should handle distinct, laser-focused jobs.
2. **Dependency Inversion**: You should retrieve `Core` functionality through explicitly defined Interfaces (`IBoardLogic`, `IDataService`) via `ServiceLocator.Get<T>()`, NOT by instantiating complex concrete types manually.
3. **Scope Segregation**: Do not put non-gameplay UI components here. Visuals here are strictly world-space gameplay implementations (Tiles, Board Generation, Visual Polish).

## Core Components
- **`GameBootstrapper`**: Governs Unity initialization execution order, instantiating services, and writing them into the app's `ServiceLocator` during `Awake()`.
- **`GameStateController`**: Drives transitioning logic across states (idle, dragging, pause menus).
- **`CompositeTile`**: User-interactable shape objects that compute internal Unity bounds to evaluate grid placement routing.
- **`TileViewRegistry`**: In-game dictionary of visual `BoardTile` and spawned instances—an isolation boundary between pure physics placement coordinates and active rendering.
