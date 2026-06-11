using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.SetMaxGuests;

public class SetMaxGuestsCommandTests
{
    // S1: Valid id and valid max guests → command created successfully
    [Theory]
    [InlineData(5)]
    [InlineData(25)]
    [InlineData(50)]
    public void GivenValidIdAndMaxGuests_WhenCreatingCommand_ThenSuccess(int count)
    {
        var result = SetMaxGuestsCommand.Create(Guid.NewGuid().ToString(), count);

        Assert.True(result is Success<SetMaxGuestsCommand>);
        var command = ((Success<SetMaxGuestsCommand>)result).Value;
        Assert.Equal(count, command.MaxGuests.Value);
    }

    // F1: Max guests below minimum (< 5) → failure with TooLow error
    [Theory]
    [InlineData(4)]
    [InlineData(0)]
    [InlineData(-5)]
    public void GivenMaxGuestsBelowMinimum_WhenCreatingCommand_ThenFailureWithTooLow(int count)
    {
        var result = SetMaxGuestsCommand.Create(Guid.NewGuid().ToString(), count);

        Assert.True(result is Failure<SetMaxGuestsCommand>);
        Assert.Contains(((Failure<SetMaxGuestsCommand>)result).Errors, e => e == EventErrors.MaxGuests.TooLow);
    }

    // F2: Max guests above maximum (> 50) → failure with TooHigh error
    [Theory]
    [InlineData(51)]
    [InlineData(100)]
    public void GivenMaxGuestsAboveMaximum_WhenCreatingCommand_ThenFailureWithTooHigh(int count)
    {
        var result = SetMaxGuestsCommand.Create(Guid.NewGuid().ToString(), count);

        Assert.True(result is Failure<SetMaxGuestsCommand>);
        Assert.Contains(((Failure<SetMaxGuestsCommand>)result).Errors, e => e == EventErrors.MaxGuests.TooHigh);
    }

    // F3: Empty guid → failure with IdEmpty error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = SetMaxGuestsCommand.Create(Guid.Empty.ToString(), 10);

        Assert.True(result is Failure<SetMaxGuestsCommand>);
        Assert.Contains(((Failure<SetMaxGuestsCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }
}