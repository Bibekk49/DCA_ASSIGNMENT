using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public record UpdateTimesBody(DateOnly StartDate, TimeOnly StartTime, DateOnly EndDate, TimeOnly EndTime);

public class UpdateTimesEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithRequest<UpdateTimesBody>.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    /// <summary>Update event start/end times (UC4)</summary>
    /// <remarks>
    /// Rules: start ≥ 08:00, duration 1–10 h, start must be in the future.
    /// Same-day: end &gt; start. Overnight (end = start+1 day): end ≤ 01:00.
    /// Reverts a READY event to DRAFT.
    /// </remarks>
    /// <param name="request">Start and end date/time</param>
    /// <response code="204">Times updated</response>
    /// <response code="400">Validation or business rule failure</response>
    [HttpPost("events/{id}/update-times")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ResultError>), StatusCodes.Status400BadRequest)]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync(UpdateTimesBody request)
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = UpdateTimesCommand.Create(id, request.StartDate, request.StartTime, request.EndDate, request.EndTime);
        if (cmdResult is Failure<UpdateTimesCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}