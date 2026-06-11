using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public class MakePrivateEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithoutRequest.AndResults<NoContent, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("events/{id}/make-private")]
    public override async Task<Results<NoContent, BadRequest<IEnumerable<ResultError>>>> HandleAsync()
    {
        var id = HttpContext.Request.RouteValues["id"]?.ToString() ?? "";
        var cmdResult = MakePrivateCommand.Create(id);
        if (cmdResult is Failure<MakePrivateCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var result = await dispatcher.DispatchAsync(cmdResult.Payload!);
        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.NoContent();
    }
}