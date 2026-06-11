using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public record SetMaxGuestsBody(int MaxGuests);

public class SetMaxGuestsEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithRequest<SetMaxGuestsBody>.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    /// <summary>Set maximum number of guests (UC7)</summary>
    /// <remarks>
    /// Valid range: 5–50. Reverts a READY event to DRAFT.
    /// When ACTIVE: only increases are allowed (cannot reduce below current value).
    /// Blocked on COMPLETED and CANCELLED events.
    /// </remarks>
    /// <param name="request">New maximum guest count (5–50)</param>
    /// <response code="204">Max guests updated</response>
    /// <response code="400">Validation or business rule failure</response>
    [HttpPost("events/{id}/set-max-guests")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ResultError>), StatusCodes.Status400BadRequest)]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync(SetMaxGuestsBody request)
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = SetMaxGuestsCommand.Create(id, request.MaxGuests);
        if (cmdResult is Failure<SetMaxGuestsCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}