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
    internal EventTimes? Times;

    
    private ViaEvent(EventId id, EventTitle eventTitle, EventDescription eventDescription, EventStatus eventStatus, EventMaxGuests eventMaxGuests ,EventVisibility eventVisibility, EventTimes? times) : base(id)
    {
        Title = eventTitle;
        Description = eventDescription;
        Status = eventStatus;
        MaxGuestNumber = eventMaxGuests;
        EventVisibility = eventVisibility;
        Times = times;
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
        
        return ResultHelper.Success(new ViaEvent(id,title, description, EventStatus.DRAFT, maxGuests,EventVisibility.PRIVATE, null));
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
    
    public Result<None> UpdateDescription(EventDescription newDescription)
    {
        if (Status == EventStatus.ACTIVE)
            return ResultHelper.Failure<None>(EventErrors.Status.CannotModifyActive);

        if (Status == EventStatus.CANCELLED)
            return ResultHelper.Failure<None>(EventErrors.Status.CannotModifyCancelled);

        if (Status == EventStatus.READY)
            Status = EventStatus.DRAFT;
 
        Description = newDescription;
 
        return ResultHelper.Success();
    }

    public Result<None> UpdateTimes(EventTimes newTimes, DateTime now)
    {
        if (Status == EventStatus.CANCELLED)
            return EventErrors.Status.CannotModifyCancelled;

        if (Status is EventStatus.ACTIVE or EventStatus.COMPLETED)
            return EventErrors.Status.CannotModifyActive;

        if (Status == EventStatus.READY)
            Status = EventStatus.DRAFT;

        var startDateTime = newTimes.StartDate.ToDateTime(newTimes.StartTime);
        if (startDateTime <= now)
            return EventErrors.Times.StartMustBeInFuture;

        Times = newTimes;


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
}
