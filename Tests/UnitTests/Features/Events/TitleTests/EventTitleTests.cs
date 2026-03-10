using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace UnitTests.ViaEvent;

public class EventTitleTests
{
    [Theory]
    [InlineData("VIA")]                  
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
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

    [Theory]
    [InlineData("a")]   
    [InlineData("XY")] 
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

    [Fact]
    public void GivenTooLongTitle_WhenCreatingTitle_ThenFailure()
    {
        // Arrange
        string titleInput = new string('A', 76); 

        // Act
        Result<EventTitle> result = EventTitle.Create(titleInput);

        // Assert
        Assert.True(result is Failure<EventTitle>);
        Assert.Contains(((Failure<EventTitle>)result).Errors, e => e == EventErrors.Title.TitleTooLong);
    }

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
