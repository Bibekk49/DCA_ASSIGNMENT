using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace ViaEventAssociation.Core.Application.Features.Event;

internal class MakePublicHandler : ICommandHandler<MakePublicCommand>
{
    private readonly IEventRepository _repo;
    private readonly IUnitOfWork _uow;

    internal MakePublicHandler(IEventRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Result> HandleAsync(MakePublicCommand command)
    {
        var evt = await _repo.GetByIdAsync(command.EventId);
        if (evt is null)
            return ResultHelper.Failure<None>(EventErrors.Event.NotFound);

        var result = evt.MakePublic();
        if (result is Failure<None>)
            return result;

        await _uow.SaveChangesAsync();
        return result;
    }
}
