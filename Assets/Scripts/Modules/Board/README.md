# Board Module (`Trigon.Board`)

Unity bridge layer that connects the pure C# core to Unity's runtime. Contains all MonoBehaviours, ScriptableObject configs, visual rendering, and input handling.

**Dependencies:** `Trigon.Core`, `Trigon.Utils`

## Structure

```
Modules/Board/
├── Components/     MonoBehaviours attached to GameObjects
│   ├── GameManager       Orchestrator: creates services, wires events, manages game flow
│   ├── BaseTile          Tile view: sprite color, sorting order, destroy animation
│   ├── BoardTile         Board cell view: holds TypeTile for prefab identity
│   ├── CompositeTile     Drag-and-drop input + placement validation + visual feedback
│   ├── BoardGenerator    Instantiates board tile prefabs, populates BoardData
│   ├── TileSpawner       Spawns random composite tile pieces in spawn zones
│   └── InputHandler      Keyboard input (Escape key → fires OnPauseRequested)
├── Data/           ScriptableObject configuration databases
│   ├── LogicConfig       Game rules: thresholds, timings, row count, frame rate
│   ├── GameViewConfig    Visual config: prefabs, colors, scales, sorting orders
│   └── ColorPalette      Color pack arrays for random tile coloring
├── Systems/        Services and coordinators (plain C# classes using Unity APIs)
│   ├── ConfigService              Provides LogicConfig + GameViewConfig to consumers
│   ├── TileViewRegistry           Maps GridCoord → view GameObjects, handles animations
│   ├── LineClearHandler           Coordinates line detection (core) + clearing animation (view)
│   └── PlayerPrefsScorePersistence  Implements IScorePersistence via PlayerPrefs
└── Bridge/         Type conversion utilities
    └── TypeConversions    GridCoord ↔ Vector3Int, Position2D ↔ Vector2
```

## Data Flow

```
Unity Input (CompositeTile drag, InputHandler keys)
    ↓
GameManager (orchestrator)
    ↓
Core Logic (BoardLogic, ScoreService, StateMachine)
    ↓
GameEvents (pure C# events)
    ↓
View Updates (TileViewRegistry, PopupManager, HudView)
```

## ScriptableObject Setup

Create these assets via **Right-click → Create → Trigon**:

| Asset | Menu Path | Assign To |
|---|---|---|
| LogicConfig | Trigon / Logic Config | GameManager |
| GameViewConfig | Trigon / Game View Config | GameManager |
| ColorPalette | Trigon / Color Palette | GameViewConfig |
