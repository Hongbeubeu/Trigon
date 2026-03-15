# Utils (`Trigon.Utils`)

Reusable utility classes and design pattern implementations. No game-specific logic.

**Dependencies:** None

## Structure

```
Utils/
└── Patterns/
    └── ServiceLocator    Lightweight IoC container for runtime dependency resolution
```

## ServiceLocator

A generic static service registry used by Unity MonoBehaviours to resolve dependencies at runtime.

**API:**
- `Register<T>(T service)` — register a service instance
- `Get<T>()` — resolve a service (logs error if not found)
- `TryGet<T>(out T service)` — safe resolution with boolean result
- `Unregister<T>()` — remove a service registration
- `Clear()` — remove all registrations

**Usage pattern:**
- Pure C# classes (Core) use **constructor injection** — no ServiceLocator
- Unity MonoBehaviours (Board, UI) use **ServiceLocator** for runtime resolution
- Services are registered in `GameManager.Awake()` and unregistered in `OnDestroy()`
