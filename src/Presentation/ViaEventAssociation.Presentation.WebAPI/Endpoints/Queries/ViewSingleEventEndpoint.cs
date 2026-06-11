using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.QueryContracts.Queries;
using ViaEventAssociation.Core.QueryContracts.QueryDispatching;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.Queries;

public record ViewSingleEventRequest([FromRoute] string Id);

public record ViewSingleEventResponse(
    string EventId,
    string Title,
    string Description,
    string? LocationName,
    string? Date,
    string? StartTime,
    string Visibility,
    int MaxGuests
);

public class ViewSingleEventEndpoint(IQueryDispatcher dispatcher, IMapper mapper)
    : ApiEndpoint.WithRequest<ViewSingleEventRequest>.AndResult<Ok<ViewSingleEventResponse>>
{
    /// <summary>Get a single event by ID</summary>
    /// <param name="request">Event GUID in the route</param>
    /// <response code="200">Event details</response>
    [HttpGet("events/{Id}")]
    [ProducesResponseType(typeof(ViewSingleEventResponse), StatusCodes.Status200OK)]
    public override async Task<Ok<ViewSingleEventResponse>> HandleAsync(ViewSingleEventRequest request)
    {
        var query = mapper.Map<ViewSingleEvent.Query>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<ViewSingleEventResponse>(answer);
        return TypedResults.Ok(response);
    }
}