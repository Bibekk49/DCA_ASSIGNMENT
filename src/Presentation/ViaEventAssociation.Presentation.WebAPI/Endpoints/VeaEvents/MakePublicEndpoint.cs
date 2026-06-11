using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public class MakePublicEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithoutRequest.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    /// <summary>Make event publicly visible (UC5)</summary>
    /// <remarks>Blocked on CANCELLED events. Allowed in DRAFT, READY, and ACTIVE.</remarks>
    /// <response code="204">Visibility set to PUBLIC</response>
    /// <response code="400">Business rule failure (e.g. event is cancelled)</response>
    [HttpPost("events/{id}/make-public")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(IEnumerable<ResultError>), StatusCodes.Status400BadRequest)]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync()
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = MakePublicCommand.Create(id);
        if (cmdResult is Failure<MakePublicCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}