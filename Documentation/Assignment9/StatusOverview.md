# Assignment 9 — Presentation Layer

## ObjectMapper (Core Tools)

| Item | Location | Status |
|---|---|---|
| `IMapper` — `Map<TOutput>(object input)` | `Core/Tools/ObjectMapper/` | ✅ |
| `IMappingConfig<TInput, TOutput>` | `Core/Tools/ObjectMapper/` | ✅ |
| `ObjectMapper` — resolves `IMappingConfig` from `IServiceProvider` | `Core/Tools/ObjectMapper/` | ✅ |

## REPR Endpoint Base Class

| Item | Details | Status |
|---|---|---|
| `ApiEndpoint` base | `ControllerBase` + `[ApiController, Route("api")]` | ✅ |
| `WithoutRequest.AndResults<TR1,TR2>` | For no-body endpoints | ✅ |
| `WithRequest<TReq>.AndResult<TR>` | For GET endpoints with route/query params | ✅ |
| `WithRequest<TReq>.AndResults<TR1,TR2>` | For POST endpoints with body | ✅ |

## Command Endpoints

| UC | Class | Route | Status |
|---|---|---|---|
| UC1 | `CreateEventEndpoint` | `POST /api/events/create-event` | ✅ |
| UC2 | `UpdateTitleEndpoint` | `POST /api/events/{id}/update-title` | ✅ |
| UC3 | `UpdateDescriptionEndpoint` | `POST /api/events/{id}/update-description` | ✅ |
| UC4 | `UpdateTimesEndpoint` | `POST /api/events/{id}/update-times` | ✅ |
| UC5 | `MakePublicEndpoint` | `POST /api/events/{id}/make-public` | ✅ |
| UC6 | `MakePrivateEndpoint` | `POST /api/events/{id}/make-private` | ✅ |
| UC7 | `SetMaxGuestsEndpoint` | `POST /api/events/{id}/set-max-guests` | ✅ |
| UC8 | `ReadyEventEndpoint` | `POST /api/events/{id}/ready` | ✅ |
| UC9 | `ActivateEventEndpoint` | `POST /api/events/{id}/activate` | ✅ |

All command endpoints: `204 No Content` on success, `400 Bad Request` with domain error array on failure.

## Query Endpoints

| Query | Class | Route | Status |
|---|---|---|---|
| View single event | `ViewSingleEventEndpoint` | `GET /api/events/{id}` | ✅ |
| Browse upcoming events | `BrowseUpcomingEventsEndpoint` | `GET /api/events` | ✅ |

`BrowseUpcomingEvents` supports: `pageNum`, `pageSize`, `titleSearch` query parameters.

## Mapping Configurations

| Config | Direction | Status |
|---|---|---|
| `ViewSingleEventRequestToQuery` | `ViewSingleEventRequest` → `ViewSingleEvent.Query` | ✅ |
| `ViewSingleEventAnswerToResponse` | `ViewSingleEvent.Answer` → `ViewSingleEventResponse` | ✅ |
| `BrowseUpcomingEventsRequestToQuery` | `BrowseUpcomingEventsRequest` → `BrowseUpcomingEvents.Query` | ✅ |
| `BrowseUpcomingEventsAnswerToResponse` | `BrowseUpcomingEvents.Answer` → `BrowseUpcomingEventsResponse` | ✅ |

## Domain Contract: ICurrentTime

| Item | Details | Status |
|---|---|---|
| `ICurrentTime` interface | `Domain/Common/Contracts/` — `GetCurrentTime()` | ✅ |
| `CurrentTimeService` | Infrastructure implementation returning `DateTime.Now` | ✅ |
| `FakeCurrentTime` | Test stub with fixed `DateTime` for deterministic tests | ✅ |
| Used by | `UpdateTimesHandler`, `ReadyEventHandler`, `ActivateEventHandler` | ✅ |

## Swagger / OpenAPI

| Item | Details | Status |
|---|---|---|
| Root `/` redirects to `/swagger` | Always available (not dev-only) | ✅ |
| `SwaggerDoc` with title, version, lifecycle description | "VIA Event Association API v1" | ✅ |
| XML doc comments on all endpoints | `<summary>`, `<remarks>`, `<response>` | ✅ |
| `[ProducesResponseType]` on all endpoints | Typed response schemas in Swagger | ✅ |
| `GenerateDocumentationFile=true` + `IncludeXmlComments` | Wires XML into Swagger UI | ✅ |

## Infrastructure DI

| Item | Status |
|---|---|
| `AddWritePersistence(connectionString)` — write DB + repository + UoW | ✅ |
| `AddQueryPersistence(connectionString)` — read DB + query handlers + query dispatcher | ✅ |
| `AddCommandDispatcher()` — command dispatcher + all 9 handlers via reflection | ✅ |
| `ICurrentTime` registered as singleton (`CurrentTimeService`) | ✅ |

## Integration Tests (WebAPI)

| Test Class | Tests | Description | Status |
|---|---|---|---|
| `CreateEventEndpointTests` | 2 | POST returns 200 with new GUID; invalid → 400 | ✅ |
| `UpdateTitleEndpointTests` | 2 | Valid title → 204; invalid → 400 | ✅ |
| `UpdateDescriptionEndpointTests` | 2 | Valid description → 204; too long → 400 | ✅ |
| `UpdateTimesEndpointTests` | 2 | Valid times → 204; past start → 400 | ✅ |
| `MakePublicEndpointTests` | 2 | Draft → 204; cancelled → 400 | ✅ |
| `MakePrivateEndpointTests` | 2 | Draft → 204; active → 400 | ✅ |
| `SetMaxGuestsEndpointTests` | 3 | Valid → 204; below min → 400; active reduce → 400 | ✅ |
| `ReadyEventEndpointTests` | 3 | Ready → 204; missing title → 400; missing times → 400 | ✅ |
| `ActivateEventEndpointTests` | 3 | Draft→active → 204; already active → 204; cancelled → 400 | ✅ |