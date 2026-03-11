using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.Features.Events.DescriptionTests;

public class EventDescriptionTests
{
    [Fact]
    public void Create_ValidDescription_ReturnsSuccess()
    {
        // Arrange
        string validDescription = "This is a valid event description.";

        // Act
        var result = EventDescription.Create(validDescription);

        // Assert
        Assert.IsType<Success<EventDescription>>(result);
        Assert.Equal(validDescription, ((Success<EventDescription>)result).Value.Value);
    }

    [Fact]
    public void Create_EmptyDescription_ReturnsSuccess()
    {
        // Arrange
        string emptyDescription = string.Empty;

        // Act
        var result = EventDescription.Create(emptyDescription);

        // Assert
        Assert.IsType<Success<EventDescription>>(result);
        Assert.Equal(string.Empty, ((Success<EventDescription>)result).Value.Value);
    }

    [Fact]
    public void Create_TooLongDescription_ReturnsFailure()
    {
        // Arrange
        string tooLongDescription = new string('a', 251);

        // Act
        var result = EventDescription.Create(tooLongDescription);

        // Assert
        Assert.IsType<Failure<EventDescription>>(result);
        var failure = (Failure<EventDescription>)result;
        Assert.Contains(EventErrors.Description.DescriptionTooLong, failure.Errors);
    }

    [Fact]
    public void Create_NullDescription_ReturnsSuccessWithEmptyString()
    {
        // Arrange
        string? nullDescription = null;

        // Act
        var result = EventDescription.Create(nullDescription);

        // Assert
        Assert.IsType<Success<EventDescription>>(result);
        Assert.Equal(string.Empty, ((Success<EventDescription>)result).Value.Value);
    }
}