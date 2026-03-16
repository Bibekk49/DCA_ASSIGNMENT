namespace DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;

public enum EventStatus
{
    CREATED,
    DRAFT,
    READY,
    ACTIVE,
    COMPLETED,
    CANCELLED
}