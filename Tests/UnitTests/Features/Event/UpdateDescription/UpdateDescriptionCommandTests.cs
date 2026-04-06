using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.UpdateDescription;

public class UpdateDescriptionCommandTests
{
    // S1: Valid eventId and valid description → command created successfully
    [Fact]
    public void GivenValidEventIdAndDescription_WhenCreatingCommand_ThenSuccess()
    {
        var result = UpdateDescriptionCommand.Create(Guid.NewGuid().ToString(), "A valid description.");

        Assert.True(result is Success<UpdateDescriptionCommand>);
        var command = ((Success<UpdateDescriptionCommand>)result).Value;
        Assert.NotNull(command.EventId);
        Assert.Equal("A valid description.", command.Description.Value);
    }

    // S2: Null or empty description → treated as empty string, success (empty is valid)
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void GivenNullOrEmptyDescription_WhenCreatingCommand_ThenSuccess(string? description)
    {
        var result = UpdateDescriptionCommand.Create(Guid.NewGuid().ToString(), description);

        Assert.True(result is Success<UpdateDescriptionCommand>);
        Assert.Equal(string.Empty, ((Success<UpdateDescriptionCommand>)result).Value.Description.Value);
    }

    // F1: Empty guid → failure with IdEmpty error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = UpdateDescriptionCommand.Create(Guid.Empty.ToString(), "Some description");

        Assert.True(result is Failure<UpdateDescriptionCommand>);
        Assert.Contains(((Failure<UpdateDescriptionCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }

    // F2: Description too long (> 250 chars) → failure with DescriptionTooLong error
    [Fact]
    public void GivenTooLongDescription_WhenCreatingCommand_ThenFailureWithDescriptionTooLong()
    {
        var longDescription = new string('a', 251);
        var result = UpdateDescriptionCommand.Create(Guid.NewGuid().ToString(), longDescription);

        Assert.True(result is Failure<UpdateDescriptionCommand>);
        Assert.Contains(((Failure<UpdateDescriptionCommand>)result).Errors, e => e == EventErrors.Description.DescriptionTooLong);
    }
}
