using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent: EntityBase<EventId>
{
    public EventTitle title { get; }
    public EventDescription description { get; }
    public EventStatus status { get; }
    public EventMaxGuests maxGuestNumber { get; }
    public EventVisibility visibility { get; }

    
    private ViaEvent(EventId id, EventStatus eventStatus, EventMaxGuests eventMaxGuests, EventTitle eventTitle, EventVisibility eventVisibility, EventDescription eventDescription) : base(id)
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
            return ResultHelper.Failure<ViaEvent>(f.Errors);

        var id = ((Success<EventId>)idResult).Value;

        Result<EventMaxGuests> maxGuest =EventMaxGuests.Create(5);

        if (maxGuest is Failure<EventMaxGuests> maxGuestFailure)
            return ResultHelper.Failure<ViaEvent>(maxGuestFailure.Errors);

        var maxGuests = ((Success<EventMaxGuests>)maxGuest).Value;

        Result<EventTitle> eventTitle = EventTitle.Create("Working Title");
        if (eventTitle is Failure<EventTitle> eventTitleFailure)
            return ResultHelper.Failure<ViaEvent>(eventTitleFailure.Errors);

        var title = ((Success<EventTitle>)eventTitle).Value;
        
        return ResultHelpers.Success(new ViaEvent(id,EventStatus.DRAFT, maxGuests, title, EventVisibility.PRIVATE));

        return ResultHelper.Success(new ViaEvent(id,EventStatus.DRAFT, maxGuests, title));
    }

    public Result<None> UpdateTitle(EventTitle newTitle)
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        if (Status is EventStatus.ACTIVE or EventStatus.COMPLETED)
            return EventErrors.Status.CannotModifyActive;

        Title = newTitle;

        if (Status == EventStatus.READY)
            Status = EventStatus.DRAFT;

        return ResultHelper.Success();
    }

    public Result<None> Cancel()
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        Status = EventStatus.CANCELLED;
        return ResultHelper.Success();
    }

    internal void SetStatusForTesting(EventStatus status)
    {
        Status = status;
    }
}
