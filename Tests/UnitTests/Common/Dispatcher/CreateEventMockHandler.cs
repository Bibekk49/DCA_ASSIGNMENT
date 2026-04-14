using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Common.Dispatcher;

public class CreateEventMockHandler : ICommandHandler<CreateEventCommand>
{
    public bool WasHandled { get; private set; }
    public CreateEventCommand? HandledCommand { get; private set; }

    public Task<Result> HandleAsync(CreateEventCommand command)
    {
        WasHandled = true;
        HandledCommand = command;
        return Task.FromResult<Result>(ResultHelper.Success());
    }
}
