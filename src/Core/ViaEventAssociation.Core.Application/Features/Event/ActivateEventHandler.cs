using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common;
using DCA_ASSIGNMENT.Core.Domain.Common.Contracts;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace ViaEventAssociation.Core.Application.Features.Event;

public class ActivateEventHandler : ICommandHandler<ActivateEventCommand>
{
    private readonly IEventRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ICurrentTime _currentTime;

    public ActivateEventHandler(IEventRepository repo, IUnitOfWork uow, ICurrentTime currentTime)
    {
        _repo = repo;
        _uow = uow;
        _currentTime = currentTime;
    }

    public async Task<Result> HandleAsync(ActivateEventCommand command)
    {
        var evt = await _repo.GetAsync(command.EventId);
        if (evt is null)
            return ResultHelper.Failure<None>(EventErrors.Event.NotFound);

        var result = evt.Activate(_currentTime.GetCurrentTime());
        if (result is Failure<None>)
            return result;

        await _uow.SaveChangesAsync();
        return result;
    }
}