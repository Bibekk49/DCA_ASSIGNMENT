using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.UpdateTimes;

public class UpdateTimesCommandTests
{
    [Fact]
    public void GivenValidInput_WhenCreatingCommand_ThenSuccess()
    {
        var result = UpdateTimesCommand.Create(
            Guid.NewGuid().ToString(),
            new DateOnly(2027, 8, 25),
            new TimeOnly(10, 0),
            new DateOnly(2027, 8, 25),
            new TimeOnly(12, 0));

        Assert.True(result is Success<UpdateTimesCommand>);
        var command = ((Success<UpdateTimesCommand>)result).Value;
        Assert.NotNull(command.EventId);
        Assert.Equal(new DateOnly(2027, 8, 25), command.Times.StartDate);
        Assert.Equal(new TimeOnly(10, 0), command.Times.StartTime);
        Assert.Equal(new DateOnly(2027, 8, 25), command.Times.EndDate);
        Assert.Equal(new TimeOnly(12, 0), command.Times.EndTime);
    }

    [Fact]
    public void GivenEmptyEventId_WhenCreatingCommand_ThenFailure()
    {
        var result = UpdateTimesCommand.Create(
            Guid.Empty.ToString(),
            new DateOnly(2027, 8, 25),
            new TimeOnly(10, 0),
            new DateOnly(2027, 8, 25),
            new TimeOnly(12, 0));

        Assert.True(result is Failure<UpdateTimesCommand>);
        Assert.Contains(((Failure<UpdateTimesCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }
}