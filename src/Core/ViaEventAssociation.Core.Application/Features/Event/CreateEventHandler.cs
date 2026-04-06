using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class CreateEventHandler: ICommandHandler<CreateEventCommand>
{
    public Task<Result> HandleAsync(CreateEventCommand command)
    {
        throw new NotImplementedException();
    }
}