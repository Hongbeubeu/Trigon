# Trigon `Generated` Assembly

This assembly holds automatically generated script files, typically produced by Unity packages (like the New Input System) or specific 3rd-party code-gen tools.

## Architectural Rules

1. **Read-Only**: Do NOT manually edit scripts inside this folder. They will be aggressively overwritten whenever the generator tools run again.
2. **Reference Point**: Other assemblies (like `Modules.Board` or `UI`) reference this assembly to consume bindings, action maps, or data classes generated here.
