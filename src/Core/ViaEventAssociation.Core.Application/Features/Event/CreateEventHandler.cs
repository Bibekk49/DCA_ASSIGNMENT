using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class CreateEventHandler: ICommandHandler<CreateEventCommand>
{
    private readonly IEventRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateEventHandler(IEventRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public Task<Result> HandleAsync(CreateEventCommand command)
    {
        var eventResult = ViaEvent.Create(command.Title, command.Description, command.MaxGuests);

        if (eventResult is Failure<ViaEvent> failure)
            return Task.FromResult<Result>(ResultHelper.Failure<None>(failure.Errors));

        var evt = ((Success<ViaEvent>)eventResult).Value;

        return HandleCreateAsync(evt);
    }

    private async Task<Result> HandleCreateAsync(ViaEvent evt)
    {
        await _repo.AddAsync(evt);
        await _uow.SaveChangesAsync();

        return ResultHelper.Success();
    }
}