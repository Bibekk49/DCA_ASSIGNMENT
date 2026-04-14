using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

public class CreateEventCommand : ICommand
{
    public EventId Id { get; }

    private CreateEventCommand(EventId id)
    {
        Id = id;
    }

    public static Result<CreateEventCommand> Create()
    {
        var idResult = EventId.New();
        if (idResult is Failure<EventId> f)
            return ResultHelper.Failure<CreateEventCommand>(f.Errors);

        return ResultHelper.Success(new CreateEventCommand(((Success<EventId>)idResult).Value));
    }
}
