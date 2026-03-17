using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;
using static DCA_ASSIGNMENT.Core.Tools.OperationResult.ResultHelper;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent : EntityBase<EventId>
{
    public EventTitle Title { get; }
    public EventDescription Description { get; }
    public EventStatus Status { get; }
    public EventMaxGuests MaxGuestNumber { get;}
    public EventVisibility Visibility { get;}
    public EventTimes? Times { get;}

    private ViaEvent(
        EventId id,
        EventTitle eventTitle,
         EventDescription eventDescription,
        EventStatus eventStatus,
        EventMaxGuests eventMaxGuests,
        EventVisibility eventVisibility,
        EventTimes? eventTimes ) : base(id)
    {
        Status = eventStatus;
        MaxGuestNumber = eventMaxGuests;
        Title = eventTitle;
        Visibility = eventVisibility;
        Description = eventDescription;
        Times = eventTimes;
    }

    public static Result<ViaEvent> Create()
    {
        var idResult = EventId.New();

        if (idResult is Failure<EventId> idFailure)
            return Failure<ViaEvent>(idFailure.Errors);

        var id = ((Success<EventId>)idResult).Value;
        return Create(id);
    }

    public static Result<ViaEvent> Create(EventId eventId)
    {
        Result<EventMaxGuests> maxGuest = EventMaxGuests.Create(5);

        if (maxGuest is Failure<EventMaxGuests> maxGuestFailure)
            return Failure<ViaEvent>(maxGuestFailure.Errors);

        var maxGuests = ((Success<EventMaxGuests>)maxGuest).Value;

        Result<EventTitle> eventTitle = EventTitle.Create("Working Title");
        if (eventTitle is Failure<EventTitle> eventTitleFailure)
            return Failure<ViaEvent>(eventTitleFailure.Errors);

        var title = ((Success<EventTitle>)eventTitle).Value;

        return ResultHelpers.Success<ViaEvent>(new ViaEvent(eventId, EventStatus.DRAFT, maxGuests, title, EventVisibility.PRIVATE, null));
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

        return ResultHelpers.Success();
    }

    public Result<None> UpdateTimes(EventTimes newTimes)
    {
        if (Status != EventStatus.DRAFT)
            return EventErrors.Times.CannotSetWhenNotDraft;

        Times = newTimes;
        return ResultHelpers.Success();
    }

    public Result<None> Cancel()
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        Status = EventStatus.CANCELLED;
        return ResultHelpers.Success();
    }

}