using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Queries;

public record BrowseUpcomingEventsRequest(
    [FromQuery] int PageNum = 1,
    [FromQuery] int PageSize = 10,
    [FromQuery] string? TitleSearch = null
);

public record BrowseUpcomingEventsResponse(
    List<BrowseUpcomingEventsResponse.EventSummary> Events,
    int TotalPages
)
{
    public record EventSummary(
        string EventId,
        string Title,
        string Description,
        string Date,
        string StartTime,
        int MaxGuests,
        string Visibility
    );
}

public class BrowseUpcomingEventsEndpoint(IQueryDispatcher dispatcher, IMapper mapper)
    : ApiEndpoint.WithRequest<BrowseUpcomingEventsRequest>.AndResult<Ok<BrowseUpcomingEventsResponse>>
{
    [HttpGet("events")]
    public override async Task<Ok<BrowseUpcomingEventsResponse>> HandleAsync(BrowseUpcomingEventsRequest request)
    {
        var query = mapper.Map<BrowseUpcomingEvents.Query>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<BrowseUpcomingEventsResponse>(answer);
        return TypedResults.Ok(response);
    }
}