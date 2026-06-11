using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public class ActivateEventEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithoutRequest.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    /// <summary>Activate an event (UC9)</summary>
    /// <remarks>
    /// Transitions DRAFT → READY → ACTIVE or READY → ACTIVE.
    /// If the event is already ACTIVE the call succeeds (idempotent).
    /// Auto-readies a DRAFT event; will fail if ready preconditions are not met.
    /// Blocked on COMPLETED and CANCELLED events.
    /// </remarks>
    /// <response code="204">Event is now ACTIVE</response>
    /// <response code="400">Precondition not met or wrong status</response>
    [HttpPost("events/{id}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ResultError>), StatusCodes.Status400BadRequest)]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync()
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = ActivateEventCommand.Create(id);
        if (cmdResult is Failure<ActivateEventCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}