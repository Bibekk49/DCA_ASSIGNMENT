# Assignment 5 — Commands & Handlers

## Shared Infrastructure

| Item | Location | Status |
|---|---|---|
| `IEventRepository` | `Domain/Aggregates/Events/IEventRepository.cs` | ✅ |
| `IUnitOfWork` | `Domain/Common/IUnitOfWork.cs` | ✅ |
| `InMemEventRepoStub` | `Tests/UnitTests/Fakes/` | ✅ |
| `FakeUoW` | `Tests/UnitTests/Fakes/` | ✅ |
| `FakeCurrentTime` | `Tests/UnitTests/Fakes/` | ✅ |

## UC1 — Create Event

| Item | Status |
|---|---|
| `CreateEventCommand` | ✅ |
| `CreateEventHandler` | ✅ |
| Aggregate tests: S1–S4 | ✅ |
| Command tests | ✅ |
| Handler tests: S1, F1 (not found) | ✅ |

## UC2 — Update Title

| Item | Status |
|---|---|
| `UpdateTitleCommand` | ✅ |
| `UpdateTitleHandler` | ✅ |
| Aggregate tests: S1–S2, F1–F4 | ✅ |
| Command tests: S1, F1–F3, F4 (empty guid) | ✅ |
| Handler tests: S1, S2 (ready→draft), F1–F4 | ✅ |

## UC3 — Update Description

| Item | Status |
|---|---|
| `UpdateDescriptionCommand` | ✅ |
| `UpdateDescriptionHandler` | ✅ |
| Aggregate tests: S1–S3, F1–F3 | ✅ |
| Command tests: S1–S3, F1, F2 (empty guid) | ✅ |
| Handler tests: S1, S2 (ready→draft), F1–F3 | ✅ |

## UC4 — Update Times

| Item | Status |
|---|---|
| `UpdateTimesCommand` | ✅ |
| `UpdateTimesHandler` (uses `ICurrentTime`) | ✅ |
| Aggregate tests: S1–S5, F1–F8 | ✅ |
| Command tests: S1–S5, F1–F8 | ✅ |
| Handler tests: S1, F1–F8, not found | ✅ |

## UC5 — Make Event Public

| Item | Status |
|---|---|
| `MakePublicCommand` | ✅ |
| `MakePublicHandler` | ✅ |
| Aggregate tests: S1, F1 | ✅ |
| Command tests: S1, F1 (empty guid) | ✅ |
| Handler tests: S1, F1 (cancelled), F2 (not found) | ✅ |

## UC6 — Make Event Private

| Item | Status |
|---|---|
| `MakePrivateCommand` | ✅ |
| `MakePrivateHandler` | ✅ |
| Aggregate tests: S1, F1, F2 | ✅ |
| Command tests: S1, F1 (empty guid) | ✅ |
| Handler tests: S1, F1 (active), F2 (cancelled), F3 (not found) | ✅ |

## UC7 — Set Max Guests

| Item | Status |
|---|---|
| `SetMaxGuestsCommand` (validates 5–50, rejects empty guid) | ✅ |
| `SetMaxGuestsHandler` | ✅ |
| Aggregate tests: S1, S2, S3, F1, F2, F3, F4, F5 | ✅ |
| Command tests: S1 (5/25/50), F1 (< 5), F2 (> 50), F3 (empty guid) | ✅ |
| Handler tests: S1, S2 (ready→draft), S3 (active increase), F1 (active reduce), F2 (not found), F3 (completed), F4 (cancelled) | ✅ |

## UC8 — Ready Event

| Item | Status |
|---|---|
| `ReadyEventCommand` (validates event id) | ✅ |
| `ReadyEventHandler` (uses `ICurrentTime`) | ✅ |
| Aggregate tests: S1, F1 (title), F1b (times), F2 (cancelled), F3 (past), F4 (active/completed) | ✅ |
| Command tests: S1 (valid guid), F1 (empty guid) | ✅ |
| Handler tests: S1, F1 (not found/title/times), F2 (cancelled), F3 (past), completed | ✅ |

## UC9 — Activate Event

| Item | Status |
|---|---|
| `ActivateEventCommand` (validates event id) | ✅ |
| `ActivateEventHandler` (uses `ICurrentTime`) | ✅ |
| Aggregate tests: S1 (draft→active), S2 (ready→active), S3 (idempotent), F1, F2, F3 | ✅ |
| Command tests: S1 (valid guid), F1 (empty guid) | ✅ |
| Handler tests: S1, S2, S3 (idempotent), F1 (not found/title/times), F2 (cancelled), completed | ✅ |

## Test Totals

| Suite | Count |
|---|---|
| Unit Tests | 199 passing |
| Integration Tests | 55 passing |
