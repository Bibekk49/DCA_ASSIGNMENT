using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public static class EventErrors
{
    public static class Id
    {
        public static readonly ResultError IdEmpty =
            new("EventId.Empty", "EventId cannot be empty.", "Validation");
    }

    public static class Title
    {
        public static readonly ResultError TitleTooShort =
            new("EventTitle.TooShort", "Event title must be between 3 and 75 characters.", "Validation");

        public static readonly ResultError TitleTooLong =
            new("EventTitle.TooLong", "Event title must be between 3 and 75 characters.", "Validation");

        public static readonly ResultError TitleEmpty =
            new("EventTitle.Empty", "Event title cannot be empty.", "Validation");
    }

    public static class Description
    {
        public static readonly ResultError DescriptionEmpty =
            new("EventDescription.Empty", "Event description cannot be empty.", "Validation");

        public static readonly ResultError DescriptionTooLong =
            new("EventDescription.TooLong", "Event description cannot exceed 1000 characters.", "Validation");
    }

    public static class MaxGuests
    {
        public static readonly ResultError TooLow =
            new("EventMaxGuests.TooLow", "Maximum guests must be at least 1.", "Validation");
    }

    public static class Event
    {
        public static readonly ResultError NotFound =
            new("Event.NotFound", "Event with the given ID was not found.", "NotFound");
    }

    public static class Status
    {
        public static readonly ResultError CannotModifyActive =
            new("Event.Status.Active", "An active event cannot be modified.", "BusinessRule");

        public static readonly ResultError CannotModifyCancelled =
            new("Event.Status.Cancelled", "A cancelled event cannot be modified.", "BusinessRule");
    }

    public static class Times
    {
        public static readonly ResultError StartDateMustBeOnOrBeforeEndDate =
            new("Event.Times.StartDateBeforeEndDate", "Start date cannot be after end date.", "Validation");

        public static readonly ResultError StartTimeMustBeBeforeEndTime =
            new("Event.Times.StartTimeBeforeEndTime", "When start and end date are the same, start time must be before end time.", "Validation");

        public static readonly ResultError StartMustBeBeforeEnd =
            new("Event.Times.StartBeforeEnd", "Start date/time range is invalid.", "Validation");

        public static readonly ResultError DurationTooShort =
            new("Event.Times.DurationTooShort", "Event duration must be at least 1 hour.", "Validation");

        public static readonly ResultError DurationTooLong =
            new("Event.Times.DurationTooLong", "Event duration cannot exceed 10 hours.", "Validation");

        public static readonly ResultError StartTooEarly =
            new("Event.Times.StartTooEarly", "Start time cannot be before 08:00.", "Validation");

        public static readonly ResultError EndTooLate =
            new("Event.Times.EndTooLate", "If ending after midnight, end time must be 01:00 or earlier.", "Validation");

       public static readonly ResultError StartMustBeInFuture =
            new("Event.Times.StartMustBeInFuture", "Event start date/time must be in the future.", "BusinessRule");
    }
}


