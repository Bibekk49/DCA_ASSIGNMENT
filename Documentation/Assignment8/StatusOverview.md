# Assignment 8 – Feature Status

* [x] QueryContracts (Core layer)
  * [x] `IQueryHandler<TQuery, TAnswer>` interface
  * [x] `IQueryDispatcher` interface
  * [x] `QueryDispatcher` implementation (resolves handlers via `IServiceProvider`)
  * [x] `ISystemTime` interface
  * [x] `BrowseUpcomingEvents` query + answer records (paging + title search)
  * [x] `ViewSingleEvent` query + answer records
  * [x] `EventsEditingOverview` query + answer records (draft / ready / cancelled)
* [x] EfcQueries (Infrastructure layer)
  * [x] Read models: `VeaEvent`, `Location`
  * [x] `ReadDbContext` with DbSets and `OnModelCreating` config
  * [x] `SeedFactory` loads JSON data from `Context/Assignment8/ViaEventAssociation/`
  * [x] `BrowseUpcomingEventsQueryHandler`
  * [x] `ViewSingleEventQueryHandler`
  * [x] `EventsEditingOverviewQueryHandler`
  * [x] `QueryDbExtensions` — DI registration for all handlers
* [x] Integration tests
  * [x] `SeedingTest` — events and locations seeded
  * [x] `BrowseUpcomingEventsQueryHandlerTests` (4 tests)
  * [x] `ViewSingleEventQueryHandlerTests` (2 tests)
  * [x] `EventsEditingOverviewQueryHandlerTests` (2 tests)
  * [x] `FakeSystemTime` for deterministic date-based tests
