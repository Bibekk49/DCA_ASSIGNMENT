using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Common.Dispatcher;

public class MakePublicMockHandler : ICommandHandler<MakePublicCommand>
{
    public bool WasHandled { get; private set; }
    public MakePublicCommand? HandledCommand { get; private set; }

    public Task<Result> HandleAsync(MakePublicCommand command)
    {
        WasHandled = true;
        HandledCommand = command;
        return Task.FromResult<Result>(ResultHelper.Success());
    }
}
