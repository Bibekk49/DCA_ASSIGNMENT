using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ViaEventAssociation.Core.Application.CommandDispaching;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Presentation.WebAPI.Endpoints.Common;

namespace ViaEventAssociation.Presentation.WebAPI.Endpoints.VeaEvents;

public class CreateEventEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithoutRequest.AndResults<Ok<string>, BadRequest<IEnumerable<ResultError>>>
{
    [HttpPost("events/create-event")]
    public override async Task<Results<Ok<string>, BadRequest<IEnumerable<ResultError>>>> HandleAsync()
    {
        var cmdResult = CreateEventCommand.Create();
        if (cmdResult is Failure<CreateEventCommand> f)
            return TypedResults.BadRequest(f.Errors);

        var command = cmdResult.Payload!;
        var result = await dispatcher.DispatchAsync(command);

        return result is Failure<None>
            ? TypedResults.BadRequest(result.GetErrors())
            : TypedResults.Ok(command.Id.Value.ToString());
    }
}