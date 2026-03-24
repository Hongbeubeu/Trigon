# Trigon `UI` Assembly

This assembly contains all interface views, menus, popups, and HUD screens.

## Architectural Rules

1. **Independent Presentation Layer**: This assembly is permitted to read game state data or observe state changes via C# Events/Actions, but it strictly MUST NOT modify the core game variables directly.
2. **Interaction via ServiceLocator**: UI components that trigger gameplay flows (like a "Restart Button" or "Pause Menu Replay") must retrieve the appropriate controller interface through `ServiceLocator.Get<T>()` or fire generic global `GameEvents`.
3. **No Gameplay Logic**: Never perform win/loss condition evaluations or score tally logic inside UI elements. The UI's single responsibility is to reflect the state to the player visually and forward player interactions back to the Board/Core.
