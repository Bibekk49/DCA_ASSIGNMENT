using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.CreateEvent;

public class CreateEventCommandTests
{
    [Fact]
    public void GivenValidInput_WhenCreatingCommand_ThenSuccess()
    {
        var result = CreateEventCommand.Create("Conference", "Annual event", 25);

        Assert.True(result is Success<CreateEventCommand>);
        var command = ((Success<CreateEventCommand>)result).Value;
        Assert.Equal("Conference", command.Title.Value);
        Assert.Equal("Annual event", command.Description.Value);
        Assert.Equal(25, command.MaxGuests.Value);
    }

    [Fact]
    public void GivenInvalidInput_WhenCreatingCommand_ThenFailure()
    {
        var result = CreateEventCommand.Create("ab", "Annual event", 0);

        Assert.True(result is Failure<CreateEventCommand>);
        var failure = (Failure<CreateEventCommand>)result;
        Assert.Contains(failure.Errors, e => e == EventErrors.Title.TitleTooShort);
        Assert.Contains(failure.Errors, e => e == EventErrors.MaxGuests.TooLow);
    }
}