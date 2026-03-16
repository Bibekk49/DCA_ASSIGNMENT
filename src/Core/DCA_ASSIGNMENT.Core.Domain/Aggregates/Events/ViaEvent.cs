using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent : IDEntity<EventId>
{
    public EventTitle title { get; }
    public EventDescription? description { get; }
    public EventStatus status { get; }
    public EventMaxGuests maxGuestNumber { get; }
    public EventVisibility visibility { get; }

    
    private ViaEvent(EventId id, EventStatus eventStatus, EventMaxGuests eventMaxGuests, EventTitle eventTitle, EventVisibility eventVisibility, EventDescription? eventDescription = null) : base(id)
    {
        status = eventStatus;
        maxGuestNumber = eventMaxGuests;
        title = eventTitle;
        visibility = eventVisibility;
        description = eventDescription;
    }

    public static Result<ViaEvent> Create()
    {
        Result<EventId> idResult = EventId.New();

        if (idResult is Failure<EventId> f)
            return ResultHelpers.Failure<ViaEvent>(f.Errors);

        var id = ((Success<EventId>)idResult).Value;
        
        Result<EventMaxGuests> maxGuest =EventMaxGuests.Create(5);
        
        if (maxGuest is Failure<EventMaxGuests> maxGuestFailure)
            return ResultHelpers.Failure<ViaEvent>(maxGuestFailure.Errors);
         
        var maxGuests = ((Success<EventMaxGuests>)maxGuest).Value;
        
        Result<EventTitle> eventTitle = EventTitle.Create("Working Title");
        if (eventTitle is Failure<EventTitle> eventTitleFailure)
            return ResultHelpers.Failure<ViaEvent>(eventTitleFailure.Errors);
        
        var title = ((Success<EventTitle>)eventTitle).Value;
        
        return ResultHelpers.Success(new ViaEvent(id,EventStatus.DRAFT, maxGuests, title, EventVisibility.PRIVATE));
    }
}