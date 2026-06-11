# Assignment 6 — Command Dispatcher

## Core Components

| Item | Location | Status |
|---|---|---|
| `ICommandDispatcher` interface | `Application/CommandDispaching/` | ✅ |
| `CommandDispatcher` (resolves `ICommandHandler<T>` from `IServiceProvider`) | `Application/CommandDispaching/` | ✅ |
| `LoggingDispatcherDecorator` (wraps dispatcher, logs before/after dispatch) | `Application/CommandDispaching/` | ✅ |
| `IServiceCollection` extension — auto-registers all handlers via reflection | `Application/Extensions/` | ✅ |

## Dispatcher Interaction Tests (ZOMBIES)

| Test | Description | Status |
|---|---|---|
| Zero | No handler registered → exception thrown | ✅ |
| One (UC2) | Correct handler registered and called | ✅ |
| One (UC5) | Correct handler registered and called | ✅ |
| One incorrect | Wrong handler registered → exception | ✅ |
| Many (UC2) | Both handlers registered, only correct one called | ✅ |
| Many (UC5) | Both handlers registered, only correct one called | ✅ |
| Same | Handler called exactly once per dispatch | ✅ |

## Logging Decorator Tests

| Test | Description | Status |
|---|---|---|
| Success path | Log written before and after successful dispatch | ✅ |
| Failure path | Log written; failure result propagated | ✅ |
| No double-dispatch | Underlying handler called exactly once | ✅ |
| Decorator wraps correctly | `ICommandDispatcher` contract satisfied | ✅ |

## Auto-Registration Tests

| Test | Description | Status |
|---|---|---|
| All 9 handlers registered | Reflection finds UC1–UC9 handlers | ✅ |
| Correct handler resolved | `IServiceProvider.GetService<ICommandHandler<T>>` returns right type | ✅ |
