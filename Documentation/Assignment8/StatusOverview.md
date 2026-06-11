# Assignment 8 — Read Side / Queries

## Query Contracts (Core Layer)

| Item | Location | Status |
|---|---|---|
| `IQueryHandler<TQuery, TAnswer>` interface | `Core/QueryContracts/` | ✅ |
| `IQueryDispatcher` interface | `Core/QueryContracts/` | ✅ |
| `QueryDispatcher` (resolves handlers via `IServiceProvider`) | `Core/QueryContracts/` | ✅ |
| `ISystemTime` interface | `Core/QueryContracts/` | ✅ |
| `BrowseUpcomingEvents` query + answer (paging + title search) | `Core/QueryContracts/Queries/` | ✅ |
| `ViewSingleEvent` query + answer | `Core/QueryContracts/Queries/` | ✅ |
| `EventsEditingOverview` query + answer (draft/ready/cancelled listing) | `Core/QueryContracts/Queries/` | ✅ |

## EfcQueries Infrastructure (Read Side)

| Item | Location | Status |
|---|---|---|
| `ReadDbContext` with `DbSet<VeaEvent>`, `DbSet<Location>` | `Infrastructure/EfcQueries/` | ✅ |
| Read model `VeaEvent` (flat, no domain logic) | `Infrastructure/EfcQueries/Models/` | ✅ |
| Read model `Location` | `Infrastructure/EfcQueries/Models/` | ✅ |
| `SeedFactory` — loads JSON seed data from `Context/Assignment8/` | `Infrastructure/EfcQueries/SeedFactories/` | ✅ |
| `SystemTimeService` — `ISystemTime` implementation | `Infrastructure/EfcQueries/` | ✅ |
| `BrowseUpcomingEventsQueryHandler` | `Infrastructure/EfcQueries/Queries/` | ✅ |
| `ViewSingleEventQueryHandler` | `Infrastructure/EfcQueries/Queries/` | ✅ |
| `EventsEditingOverviewQueryHandler` | `Infrastructure/EfcQueries/Queries/` | ✅ |
| `QueryDbExtensions.AddQueryPersistence(connectionString)` | `Infrastructure/EfcQueries/` | ✅ |

## Integration Tests

| Test Class | Tests | Description | Status |
|---|---|---|---|
| `SeedingTest` | 1 | Events and locations seeded correctly | ✅ |
| `BrowseUpcomingEventsQueryHandlerTests` | 4 | Paging, title search, future-only filter | ✅ |
| `ViewSingleEventQueryHandlerTests` | 2 | By ID — found and not-found | ✅ |
| `EventsEditingOverviewQueryHandlerTests` | 2 | Returns draft/ready/cancelled events | ✅ |
| `FakeSystemTime` | — | Deterministic "now" for date-based tests | ✅ |

## Architecture Notes

- Separate `ReadDbContext` from the write-side `EfcDbContext` — CQRS separation
- Read models are plain flat classes with no domain logic or value objects
- Seed data is loaded from JSON files in `Context/Assignment8/ViaEventAssociation/`
- `ISystemTime` used by query handlers instead of `DateTime.Now` for testability