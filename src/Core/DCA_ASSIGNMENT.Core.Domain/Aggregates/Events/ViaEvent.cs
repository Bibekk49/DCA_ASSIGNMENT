using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public class ViaEvent : IDEntity<EventId>
{
    public EventTitle title { get; }
    public EventStatus status { get; private set; }
    public EventMaxGuests maxGuestNumber { get; }
    public EventDescription description { get; private set; }

    private ViaEvent(
        EventId id,
        EventStatus eventStatus,
        EventMaxGuests eventMaxGuests,
        EventTitle eventTitle,
        EventDescription eventDescription) : base(id)
    {
        status = eventStatus;
        maxGuestNumber = eventMaxGuests;
        title = eventTitle;
        description = eventDescription;
    }

    public static Result<ViaEvent> Create()
    {
        Result<EventId> idResult = EventId.New();

        if (idResult is Failure<EventId> f)
            return ResultHelpers.Failure<ViaEvent>(f.Errors);

        var id = ((Success<EventId>)idResult).Value;

        Result<EventMaxGuests> maxGuest = EventMaxGuests.Create(5);

        if (maxGuest is Failure<EventMaxGuests> maxGuestFailure)
            return ResultHelpers.Failure<ViaEvent>(maxGuestFailure.Errors);

        var maxGuests = ((Success<EventMaxGuests>)maxGuest).Value;

        Result<EventTitle> eventTitle = EventTitle.Create("Working Title");

        if (eventTitle is Failure<EventTitle> eventTitleFailure)
            return ResultHelpers.Failure<ViaEvent>(eventTitleFailure.Errors);

        var title = ((Success<EventTitle>)eventTitle).Value;

        Result<EventDescription> eventDescription = EventDescription.Create(string.Empty);

        if (eventDescription is Failure<EventDescription> eventDescriptionFailure)
            return ResultHelpers.Failure<ViaEvent>(eventDescriptionFailure.Errors);

        var description = ((Success<EventDescription>)eventDescription).Value;

        return ResultHelpers.Success(new ViaEvent(id, EventStatus.DRAFT, maxGuests, title, description));
    }

    public Result<None> UpdateTitle(EventTitle newTitle)
    {
        Result<EventTitle> title = EventTitle.Create(newTitle.Value);

        if (title is Failure<EventTitle> titleFailure)
            return ResultHelpers.Failure<None>(titleFailure.Errors);

        return ResultHelpers.Success();
    }

    public Result<None> UpdateDescription(EventDescription newDescription)
    {
        if (status == EventStatus.ACTIVE)
            return ResultHelpers.Failure<None>(EventErrors.Status.CannotModifyActive);

        if (status == EventStatus.CANCELLED)
            return ResultHelpers.Failure<None>(EventErrors.Status.CannotModifyCancelled);

        description = newDescription;

        if (status == EventStatus.READY)
            status = EventStatus.DRAFT;

        return ResultHelpers.Success();
    }
}