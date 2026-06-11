# Assignment 9 — Presentation Layer Status

## What was implemented

### 1. ObjectMapper — `src/Core/Tools/ViaEventAssociation.Core.Tools.ObjectMapper/`
- `IMapper` — `Map<TOutput>(object input)` interface
- `IMappingConfig<TInput, TOutput>` — typed mapping config interface
- `ObjectMapper` — resolves `IMappingConfig` from `IServiceProvider`; falls back to JSON round-trip for identical structures

### 2. REPR Fluent Base Class — `Endpoints/Common/ApiEndpoint.cs`
- `EndpointBase : ControllerBase` with `[ApiController, Route("api")]`
- `ApiEndpoint.WithoutRequest.AndResult<TR>` / `AndResults<TR1,TR2>`
- `ApiEndpoint.WithRequest<TReq>.AndResult<TR>` / `AndResults<TR1,TR2>` / `AndResults<TR1,TR2,TR3>`

### 3. Command Endpoints — `Endpoints/VeaEvents/`
| UC  | Endpoint class             | Route                             |
|-----|---------------------------|-----------------------------------|
| UC1 | CreateEventEndpoint        | POST  /api/events/create-event    |
| UC2 | UpdateTitleEndpoint        | POST  /api/events/{id}/update-title |
| UC3 | UpdateDescriptionEndpoint  | POST  /api/events/{id}/update-description |
| UC4 | UpdateTimesEndpoint        | POST  /api/events/{id}/update-times |
| UC5 | MakePublicEndpoint         | POST  /api/events/{id}/make-public |
| UC6 | MakePrivateEndpoint        | POST  /api/events/{id}/make-private |

### 4. Query Endpoints — `Endpoints/Queries/`
| Query                | Route              |
|----------------------|--------------------|
| ViewSingleEvent      | GET /api/events/{id} |
| BrowseUpcomingEvents | GET /api/events      |

### 5. Mapping Configurations — `MappingConfigurations/`
- `ViewSingleEventRequestToQuery`
- `ViewSingleEventAnswerToResponse`
- `BrowseUpcomingEventsRequestToQuery`
- `BrowseUpcomingEventsAnswerToResponse`

### 6. Infrastructure fixes
- `EfcDbContext` updated to use `DbContextOptions<EfcDbContext>` (generic) to avoid DI ambiguity
- `EfcDmPersistenceExtensions.AddWritePersistence(connectionString)` — registers write DB + repository + UoW
- `QueryDbExtensions.AddQueryPersistence(connectionString)` — registers read DB + query handlers + query dispatcher
- `ApplicationExtensions.AddCommandDispatcher()` — registers command dispatcher + all handlers via reflection