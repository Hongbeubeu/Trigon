# Trigon `Utils` Assembly

This assembly contains globally sharable abstractions, design patterns, and generalized helpers.

## Architectural Rules

1. **Maximum Reusability**: Classes here (like `ServiceLocator`, extensions, formatters) must be completely agnostic to the specific rules of the Trigon game.
2. **Global Access**: Since utilities are referenced universally across `Modules` and `UI`, code here must be completely side-effect free and thread-safe where possible.
3. **No Cross-Talk**: Code in `Utils` cannot reference `Core`, `Board`, or `UI` directly. It sits at the absolute foundation (alongside `Core`) and strictly provides scaffolding structure for the application.
