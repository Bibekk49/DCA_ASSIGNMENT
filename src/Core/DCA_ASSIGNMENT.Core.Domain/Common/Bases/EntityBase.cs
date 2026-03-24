namespace DCA_ASSIGNMENT.Core.Domain.Common.Bases;

public abstract class EntityBase<TId>(TId id)
{
    public TId Id { get; } = id;
}