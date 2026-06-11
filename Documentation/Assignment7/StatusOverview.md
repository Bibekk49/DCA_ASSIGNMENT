# Assignment 7 — Persistence (EFC, Repository, UoW)

## Project Setup

| Item | Details | Status |
|---|---|---|
| Solution folder `Infrastructure/` | Created under `src/` | ✅ |
| `ViaEventAssociation.Infrastructure.EfcDmPersistence` | Class Library project | ✅ |
| NuGet: `Microsoft.EntityFrameworkCore.Sqlite` v9 | Write-side database | ✅ |
| NuGet: `Microsoft.EntityFrameworkCore.Design` v9 | Enables `dotnet ef` tooling | ✅ |

## DbContext

| Item | Details | Status |
|---|---|---|
| `EfcDbContext` | `OnModelCreating` + `ApplyConfigurationsFromAssembly` | ✅ |
| `DbSet<ViaEvent> Events` | Maps the aggregate root | ✅ |
| `DesignTimeContextFactory` | Enables `dotnet ef dbcontext script` | ✅ |
| EF Migration | `InitialCreate` migration generated | ✅ |

## EFC Entity Configuration

| Item | Details | Status |
|---|---|---|
| `VeaEventEntityConfiguration` | `IEntityTypeConfiguration<ViaEvent>` | ✅ |
| `HasConversion` for `EventId` | GUID stored as string | ✅ |
| `HasConversion` for `EventTitle` | Stored as string | ✅ |
| `HasConversion` for `EventDescription` | Stored as string | ✅ |
| `HasConversion` for `EventMaxGuests` | Stored as int | ✅ |
| `HasConversion` for `EventStatus` / `EventVisibility` | Stored as string | ✅ |
| `OwnsOne<EventTimes>` | 4 nullable columns: StartDate, StartTime, EndDate, EndTime | ✅ |
| `UsePropertyAccessMode(Field)` | Maps private backing fields | ✅ |

## Domain Changes for EFC Compatibility

| Item | Details | Status |
|---|---|---|
| `ViaEvent` private no-arg constructor | Required by EFC | ✅ |
| `EntityBase<TId>` explicit constructors | `Id { get; private set; }` | ✅ |
| `EventTimes` properties `{ get; init; }` + no-arg constructor | Required by `OwnsOne` | ✅ |
| `Reconstitute(...)` factory methods on all value objects | Bypass validation on load | ✅ |

## Repository & Unit of Work

| Item | Location | Status |
|---|---|---|
| `IGenericRepository<TAgg, TId>` | `Domain/Common/` | ✅ |
| `IEventRepository : IGenericRepository<ViaEvent, EventId>` | `Domain/Aggregates/Events/` | ✅ |
| `RepositoryBase<TAgg, TId>` (abstract, all methods virtual) | `Infrastructure/` | ✅ |
| `VeaEventEfcRepository` (overrides `GetAsync` with `SingleOrDefaultAsync`) | `Infrastructure/VeaEventPersistence/` | ✅ |
| `SqliteUnitOfWork` | `Infrastructure/UnitOfWork/` | ✅ |
| DI extension `AddWritePersistence(connectionString)` | `EfcDmPersistenceExtensions.cs` | ✅ |

## Integration Tests

| Test | Description | Status |
|---|---|---|
| `VeaEventContextConfigurationTests` (3 tests) | Verifies EFC column mappings | ✅ |
| `VeaEventRepositoryTests` (7 tests) | Save + load event in multiple states | ✅ |
| Real SQLite DB used (no mocks) | DbContext disposed between save and load | ✅ |