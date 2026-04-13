using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using UnitTests.Fakes;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;
using ViaEventAssociation.Core.Application.Features.Event;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.Features.Event.UpdateTimes;

public class UpdateTimesHandlerTests
{
    [Fact]
    public async Task GivenExistingDraftEvent_WhenHandlingUpdateTimesCommand_ThenTimesAreUpdated()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTimesCommand> handler = new UpdateTimesHandler(repo, uow);

        var evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        await repo.AddAsync(evt);

        var command = ((Success<UpdateTimesCommand>)UpdateTimesCommand.Create(
            evt.Id.Value.ToString(),
            new DateOnly(2030, 8, 25),
            new TimeOnly(10, 0),
            new DateOnly(2030, 8, 25),
            new TimeOnly(12, 0))).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Success<None>);
        Assert.NotNull(repo.Events[0].Times);
        Assert.Equal(new DateOnly(2030, 8, 25), repo.Events[0].Times!.StartDate);
        Assert.Equal(new TimeOnly(10, 0), repo.Events[0].Times!.StartTime);
    }

    [Fact]
    public async Task GivenMissingEvent_WhenHandlingUpdateTimesCommand_ThenFailure()
    {
        var repo = new InMemEventRepoStub();
        var uow = new FakeUoW();
        ICommandHandler<UpdateTimesCommand> handler = new UpdateTimesHandler(repo, uow);

        var command = ((Success<UpdateTimesCommand>)UpdateTimesCommand.Create(
            Guid.NewGuid().ToString(),
            new DateOnly(2030, 8, 25),
            new TimeOnly(10, 0),
            new DateOnly(2030, 8, 25),
            new TimeOnly(12, 0))).Value;

        var result = await handler.HandleAsync(command);

        Assert.True(result is Failure<None>);
        Assert.Contains(((Failure<None>)result).Errors, e => e == EventErrors.Event.NotFound);
    }
}