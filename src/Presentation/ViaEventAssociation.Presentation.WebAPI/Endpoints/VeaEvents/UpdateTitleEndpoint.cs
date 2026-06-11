using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public record UpdateTitleBody(string Title);

public class UpdateTitleEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithRequest<UpdateTitleBody>.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("events/{id}/update-title")]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync(UpdateTitleBody request)
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = UpdateTitleCommand.Create(id, request.Title);
        if (cmdResult is Failure<UpdateTitleCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}
