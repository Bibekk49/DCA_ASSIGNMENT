using DCA_ASSIGNMENT.Core.Domain.Common.Bases;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;


public class ViaEvent : IDEntity<EventId>
{

    private ViaEvent(EventId id, EventStatus status, EventMaxGuests maxGuests) : base(id)
    {
        Status = status;
        MaxGuests = maxGuests;
    }

    public EventStatus Status { get; }
    public EventMaxGuests MaxGuests { get; }

    public static Result<ViaEvent> Create()
    {
        Result<EventId> idResult = EventId.New();

        if (idResult is Failure<EventId> idFailure)
            return ResultHelpers.Failure<ViaEvent>(idFailure.Errors);

        var id = ((Success<EventId>)idResult).Value;

        Result<EventMaxGuests> maxGuestsResult = EventMaxGuests.Create(5);

        if (maxGuestsResult is Failure<EventMaxGuests> maxGuestsFailure)
            return ResultHelpers.Failure<ViaEvent>(maxGuestsFailure.Errors);

        var maxGuests = ((Success<EventMaxGuests>)maxGuestsResult).Value;

        return ResultHelpers.Success(new ViaEvent(id, EventStatus.Draft, maxGuests));
    }
}