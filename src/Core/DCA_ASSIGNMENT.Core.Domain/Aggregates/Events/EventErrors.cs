using DCA_ASSIGNMENT.Core.Tools.OperationResult;

namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
public static class EventErrors
{
    public static class Title
    {
        public static readonly ResultError TitleTooShort =
            new("EventTitle.TooShort", "Event title must be between 3 and 75 characters.", "Validation");

        public static readonly ResultError TitleTooLong =
            new("EventTitle.TooLong", "Event title must be between 3 and 75 characters.", "Validation");

        public static readonly ResultError TitleEmpty =
            new("EventTitle.Empty", "Event title cannot be empty.", "Validation");
    }

    public static class Status
    {
        public static readonly ResultError CannotModifyActive =
            new("Event.Status.Active", "An active event cannot be modified.", "BusinessRule");

        public static readonly ResultError CannotModifyCancelled =
            new("Event.Status.Cancelled", "A cancelled event cannot be modified.", "BusinessRule");
    }
}