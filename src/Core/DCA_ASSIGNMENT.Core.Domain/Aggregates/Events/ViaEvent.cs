using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent : EntityBase<EventId>
{
    internal EventTitle Title;
    internal EventDescription Description;
    internal EventStatus Status;
    internal EventMaxGuests MaxGuestNumber;
    internal EventVisibility EventVisibility;

    private ViaEvent(EventId id, EventTitle eventTitle, EventDescription eventDescription, EventStatus eventStatus,
        EventMaxGuests eventMaxGuests, EventVisibility eventVisibility) : base(id)
    {
        Title = eventTitle;
        Description = eventDescription;
        Status = eventStatus;
        MaxGuestNumber = eventMaxGuests;
        EventVisibility = eventVisibility;
    }

    public static Result<ViaEvent> Create()
    {
        Result<EventId> idResult = EventId.New();

        if (idResult is Failure<EventId> f)
            return ResultHelper.Failure<ViaEvent>(f.Errors);

        var id = ((Success<EventId>)idResult).Value;

        Result<EventMaxGuests> maxGuest = EventMaxGuests.Create(5);

        if (maxGuest is Failure<EventMaxGuests> maxGuestFailure)
            return ResultHelper.Failure<ViaEvent>(maxGuestFailure.Errors);

        var maxGuests = ((Success<EventMaxGuests>)maxGuest).Value;

        Result<EventTitle> eventTitle = EventTitle.Create("Working Title");
        if (eventTitle is Failure<EventTitle> eventTitleFailure)
            return ResultHelper.Failure<ViaEvent>(eventTitleFailure.Errors);

        var title = ((Success<EventTitle>)eventTitle).Value;

        Result<EventDescription> eventDescription = EventDescription.Create("");
        if (eventDescription is Failure<EventDescription> eventDescriptionFailure)
            return ResultHelper.Failure<ViaEvent>(eventDescriptionFailure.Errors);

        var description = ((Success<EventDescription>)eventDescription).Value;

        return ResultHelper.Success(new ViaEvent(id, title, description, EventStatus.DRAFT, maxGuests,
            EventVisibility.PRIVATE));
    }

    public Result<None> UpdateTitle(EventTitle newTitle)
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        if (Status is EventStatus.ACTIVE or EventStatus.COMPLETED)
            return EventErrors.Status.CannotModifyActive;


        if (Status == EventStatus.READY)
            Status = EventStatus.DRAFT;

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

    public Result<None> MakePublic()
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        EventVisibility = EventVisibility.PUBLIC;
        return ResultHelper.Success();
    }

    internal void SetStatusForTesting(EventStatus status)
    {
        Status = status;
    }
}