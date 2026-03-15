# UI (`Trigon.UI`)

UI framework and implementations. Fully decoupled from the Board module — communicates only through `GameEvents`.

**Dependencies:** `Trigon.Core`, `Unity.TextMeshPro`

## Structure

```
UI/
├── Common/         Reusable popup framework
│   ├── BasePopup         Abstract MonoBehaviour: CanvasGroup show/hide
│   ├── BasePopupData     Abstract ScriptableObject: popup static config (title, etc.)
│   └── PopupManager      Auto-discovers child popups, manages show/hide stack,
│                          listens to OnGameStateChanged for state-driven popups
├── Data/           UI configuration
│   └── UIViewConfig      ScriptableObject: score display format
├── Hud/            Gameplay heads-up display
│   └── HudView           Score texts, max score, pause button, hides during popups
└── Popups/         Concrete popup implementations (each with own data + logic)
    ├── PausePopup        Continue / Replay / Quit buttons
    ├── PausePopupData    ScriptableObject: title, button labels
    ├── GameOverPopup     Displays final score, Replay / Quit buttons
    └── GameOverPopupData ScriptableObject: title, score format, button labels
```

## Event Communication

UI never references the Board module directly. All communication goes through events:

| UI Action | Event Fired | Handled By |
|---|---|---|
| Pause button (HUD) | `OnPauseRequested` | GameManager |
| Continue button (PausePopup) | `OnResumeRequested` | GameManager |
| Replay button (any popup) | `OnReplayRequested` | GameManager |
| State changes | `OnGameStateChanged` | PopupManager, HudView |
| Score updates | `OnScoreChanged` | HudView, GameOverPopup |

## Adding a New Popup

1. Create `NewPopupData : BasePopupData` ScriptableObject with config fields
2. Create `NewPopup : BasePopup` MonoBehaviour with serialized data + UI references
3. Place as a child of the `PopupManager` GameObject in the scene
4. Show it via `PopupManager.Show<NewPopup>()` or add a state mapping in `PopupManager`

## ScriptableObject Setup

| Asset | Menu Path | Assign To |
|---|---|---|
| UIViewConfig | Trigon / UI View Config | HudView |
| PausePopupData | Trigon / UI / Pause Popup Data | PausePopup |
| GameOverPopupData | Trigon / UI / Game Over Popup Data | GameOverPopup |
