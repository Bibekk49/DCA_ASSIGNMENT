using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent: EntityBase<EventId>
{
    public EventTitle Title { get; private set; }

    public EventStatus Status { get; private set; }
    public EventMaxGuests MaxGuestNumber { get; private set; }

    
    private ViaEvent(EventId id, EventStatus eventStatus, EventMaxGuests eventMaxGuests, EventTitle eventTitle) : base(id)
    {
        Status = eventStatus;
        MaxGuestNumber = eventMaxGuests;
        Title = eventTitle;
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

        return ResultHelper.Success(new ViaEvent(id,EventStatus.DRAFT, maxGuests, title));
    }

    public Result<None> UpdateTitle(EventTitle newTitle)
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        if (Status != EventStatus.DRAFT)
            return EventErrors.Status.CannotModifyActive;

        Title = newTitle;
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
