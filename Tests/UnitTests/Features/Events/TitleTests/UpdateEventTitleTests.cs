using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using DomainEvent = DCA_ASSIGNMENT.Core.Domain.Aggregates.Events.ViaEvent;

namespace UnitTests.ViaEvent;

public class UpdateEventTitleTests
{
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void GivenDraftEventAndValidTitle_WhenUpdatingTitle_ThenTitleIsUpdated(string titleStr)
    {
        // Arrange
        DomainEvent evt = ((Success<DomainEvent>)DomainEvent.Create()).Value;
        EventTitle newTitle = ((Success<EventTitle>)EventTitle.Create(titleStr)).Value;

        // Act
        Result<None> result = evt.UpdateTitle(newTitle);

        // Assert
        Assert.True(result is Success<None>);
        Assert.Equal(titleStr, evt.Title!.Value);
    }
}
