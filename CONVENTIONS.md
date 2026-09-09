# Unity Project Development Guidelines

## 1. Architecture & Dependency Injection (VContainer)
- **Multi-Scope Hierarchy**: The architecture relies on multiple VContainer `LifetimeScope` boundaries (e.g., Project, Feature, UI, Level).
- **Registration over Singletons**: Never use Singletons or static instances. Dependencies must be registered in the appropriate `LifetimeScope` and injected via Constructors or `[Inject]` methods.
- **Dependency Resolution**: Inject `IObjectResolver` or usage-specific factories when runtime creation/resolution is required rather than instantiating classes directly with `new`.

## 2. State Machine & Execution Flow
- **State Lifecycle**: All GameStates (`IGameState`) must follow strict `Enter()`, `Tick()`, and `Exit()` flows.
- **Allocation Rules**: Avoid allocating new State instances during state transitions. Pre-register states within DI or reuse instances via pooling/dictionary resolution.

## 3. WebGL & Performance Optimization
- **Zero-Allocation Hot Paths**: Minimize Garbage Collection triggers. Prohibit `new` allocations inside `Update()`, `Tick()`, or loops.
- **Async Handling**: Use `UniTask` / `UniTaskVoid` instead of standard `System.Threading.Tasks.Task` or Coroutines.
- **Data Collections**: Use pooled collections (`ListPool<T>`, `DictionaryPool<TKey, TValue>`) for temporary computations in hot code paths.

## 4. AI Agent Workflow Protocols
- **Context Awareness**: Inspect active `LifetimeScope` files, Interfaces (`I*.cs`), and related systems before suggesting architecture.
- **Interactive Proposals**: Provide brief architectural approaches first. Generate code diffs strictly after confirmation.
- **Minimal Diffs**: Output only modified methods and essential code changes. Avoid printing full class boilerplates or untouched code.
