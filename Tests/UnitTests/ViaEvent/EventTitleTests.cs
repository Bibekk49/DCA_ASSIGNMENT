using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.ViaEvent;

public class EventTitleTests
{
    // UC2.S1 + S2 — Valid titles between 3 and 75 chars
    [Theory]
    [InlineData("VIA")]                  // boundary: exactly 3 (min)
    [InlineData("Scary Movie Night!")]   // from requirements
    [InlineData("Graduation Gala")]      // from requirements
    [InlineData("VIA Hackathon")]        // from requirements
    public void GivenValidTitle_WhenCreatingTitle_ThenSuccess(string input)
    {
        // Arrange
        string titleInput = input;

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Success<EventTitle>);
        Assert.Equal(titleInput, ((Success<EventTitle>)result).Value.Value);
    }

    // UC2.S1 + S2 — boundary: exactly 75 chars (max)
    [Fact]
    public void GivenTitleWithExactly75Chars_WhenCreatingTitle_ThenSuccess()
    {
        // Arrange
        string titleInput = new string('A', 75);

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Success<EventTitle>);
    }

    // UC2.F1 — Title is 0 characters (empty string)
    [Fact]
    public void GivenEmptyTitle_WhenCreatingTitle_ThenFailure()
    {
        // Arrange
        string titleInput = "";

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Failure<EventTitle>);
        Assert.Contains(((Failure<EventTitle>)result).Errors, e => e == EventErrors.Title.TitleEmpty);
    }

    // UC2.F2 — Title is too short (less than 3 chars)
    [Theory]
    [InlineData("a")]   // 1 char
    [InlineData("XY")]  // 2 chars — boundary: one below min
    public void GivenTooShortTitle_WhenCreatingTitle_ThenFailure(string input)
    {
        // Arrange
        string titleInput = input;

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Failure<EventTitle>);
        Assert.Contains(((Failure<EventTitle>)result).Errors, e => e == EventErrors.Title.TitleTooShort);
    }

    // UC2.F3 — Title is too long (more than 75 chars)
    [Fact]
    public void GivenTooLongTitle_WhenCreatingTitle_ThenFailure()
    {
        // Arrange
        string titleInput = new string('A', 76); // boundary: one above max

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Failure<EventTitle>);
        Assert.Contains(((Failure<EventTitle>)result).Errors, e => e == EventErrors.Title.TitleTooLong);
    }

    // UC2.F4 — Title is null
    [Fact]
    public void GivenNullTitle_WhenCreatingTitle_ThenFailure()
    {
        // Arrange
        string? titleInput = null;

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Failure<EventTitle>);
        Assert.Contains(((Failure<EventTitle>)result).Errors, e => e == EventErrors.Title.TitleEmpty);
    }
}
