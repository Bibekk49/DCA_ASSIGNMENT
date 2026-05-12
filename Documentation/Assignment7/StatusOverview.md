# Assignment 7 – Feature Status

* [x] Solution setup
  * [x] `Infrastructure/` solution folder created
  * [x] Class Library project `ViaEventAssociation.Infrastructure.EfcDmPersistence`
  * [x] NuGet packages: `Microsoft.EntityFrameworkCore`, `.Design`, `.Sqlite` (v9.0.4)
  * [x] Project references Domain
* [x] DbContext
  * [x] `DmContext` with `OnModelCreating` + `ApplyConfigurationsFromAssembly`
  * [x] `DbSet<ViaEvent> Events`
  * [x] `DesignTimeContextFactory`
* [x] EFC Configuration
  * [x] `VeaEventEntityConfiguration` — `HasConversion` for all value objects
  * [x] `OwnsOne<EventTimes>` — 4 nullable columns (StartDate, StartTime, EndDate, EndTime)
  * [x] `HasConversion<string>` for `EventStatus` and `EventVisibility` enums
  * [x] `UsePropertyAccessMode(Field)` for internal fields
* [x] Domain changes for EFC compatibility
  * [x] `EntityBase<TId>` — explicit constructors, `Id { get; private set; }`
  * [x] `ViaEvent` — private no-arg constructor
  * [x] `EventTimes` — properties `{ get; init; }`, private no-arg constructor
  * [x] `EventId`, `EventTitle`, `EventDescription`, `EventMaxGuests` — `Reconstitute(...)` factory methods
  * [x] `InternalsVisibleTo` added for infrastructure + integration test projects
* [x] Repository
  * [x] `IGenericRepository<TAgg, TId>` in `Domain/Common/`
  * [x] `IEventRepository : IGenericRepository<ViaEvent, EventId>` (updated)
  * [x] `RepositoryBase<TAgg, TId>` abstract class in infrastructure
  * [x] `VeaEventEfcRepository` — overrides `GetAsync` with `SingleOrDefaultAsync`
* [x] Unit of Work
  * [x] `SqliteUnitOfWork` implementing `IUnitOfWork`
* [x] Integration Tests
  * [x] `IntegrationTests` project under `Tests/`
  * [x] `GlobalUsings.cs`
  * [x] `DmContextConfiguration/VeaEventContextConfigurationTests.cs` (3 tests)
  * [x] `Repositories/VeaEventRepositoryTests.cs` (7 tests)
  * [x] All 10 integration tests passing
