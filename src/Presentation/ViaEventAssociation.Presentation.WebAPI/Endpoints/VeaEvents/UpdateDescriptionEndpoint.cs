using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public record UpdateDescriptionBody(string Description);

public class UpdateDescriptionEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithRequest<UpdateDescriptionBody>.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("events/{id}/update-description")]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync(UpdateDescriptionBody request)
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = UpdateDescriptionCommand.Create(id, request.Description);
        if (cmdResult is Failure<UpdateDescriptionCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}
