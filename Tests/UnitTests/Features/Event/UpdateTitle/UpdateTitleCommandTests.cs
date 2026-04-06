using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using ViaEventAssociation.Core.Application.CommandDispaching.Commands.Event;

namespace UnitTests.Features.Event.UpdateTitle;

public class UpdateTitleCommandTests
{
    // S1: Valid eventId and valid title → command created successfully
    [Fact]
    public void GivenValidEventIdAndTitle_WhenCreatingCommand_ThenSuccess()
    {
        var result = UpdateTitleCommand.Create(Guid.NewGuid().ToString(), "Valid Title");

        Assert.True(result is Success<UpdateTitleCommand>);
        var command = ((Success<UpdateTitleCommand>)result).Value;
        Assert.NotNull(command.EventId);
        Assert.Equal("Valid Title", command.Title.Value);
    }

    // F1: Title too short → failure with TitleTooShort error
    [Theory]
    [InlineData("ab")]
    [InlineData("x")]
    public void GivenTooShortTitle_WhenCreatingCommand_ThenFailureWithTitleTooShort(string title)
    {
        var result = UpdateTitleCommand.Create(Guid.NewGuid().ToString(), title);

        Assert.True(result is Failure<UpdateTitleCommand>);
        Assert.Contains(((Failure<UpdateTitleCommand>)result).Errors, e => e == EventErrors.Title.TitleTooShort);
    }

    // F2: Empty guid → failure with IdEmpty error
    [Fact]
    public void GivenEmptyGuid_WhenCreatingCommand_ThenFailureWithIdError()
    {
        var result = UpdateTitleCommand.Create(Guid.Empty.ToString(), "Valid Title");

        Assert.True(result is Failure<UpdateTitleCommand>);
        Assert.Contains(((Failure<UpdateTitleCommand>)result).Errors, e => e == EventErrors.Id.IdEmpty);
    }

    // F3: Title too long (> 75 chars) → failure with TitleTooLong error
    [Fact]
    public void GivenTooLongTitle_WhenCreatingCommand_ThenFailureWithTitleTooLong()
    {
        var longTitle = new string('a', 76);
        var result = UpdateTitleCommand.Create(Guid.NewGuid().ToString(), longTitle);

        Assert.True(result is Failure<UpdateTitleCommand>);
        Assert.Contains(((Failure<UpdateTitleCommand>)result).Errors, e => e == EventErrors.Title.TitleTooLong);
    }

    // F4: Null/empty title → failure with TitleEmpty error
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenNullOrEmptyTitle_WhenCreatingCommand_ThenFailureWithTitleEmpty(string? title)
    {
        var result = UpdateTitleCommand.Create(Guid.NewGuid().ToString(), title!);

        Assert.True(result is Failure<UpdateTitleCommand>);
        Assert.Contains(((Failure<UpdateTitleCommand>)result).Errors, e => e == EventErrors.Title.TitleEmpty);
    }
}
