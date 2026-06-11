using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public class ReadyEventEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithoutRequest.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    /// <summary>Ready an event for activation (UC8)</summary>
    /// <remarks>
    /// Transitions the event from DRAFT → READY.
    /// Requires: title ≠ "Working Title", times set, and start time in the future.
    /// Blocked on ACTIVE, COMPLETED, and CANCELLED events.
    /// </remarks>
    /// <response code="204">Event is now READY</response>
    /// <response code="400">Precondition not met (missing fields, past start time, wrong status)</response>
    [HttpPost("events/{id}/ready")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ResultError>), StatusCodes.Status400BadRequest)]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync()
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = ReadyEventCommand.Create(id);
        if (cmdResult is Failure<ReadyEventCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}