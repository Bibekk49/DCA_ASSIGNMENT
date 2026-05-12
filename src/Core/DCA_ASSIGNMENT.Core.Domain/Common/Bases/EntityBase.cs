namespace DCA_ASSIGNMENT.Core.Domain.Common.Bases;

public abstract class EntityBase<TId>
{
    public TId Id { get; private set; } = default!;

    protected EntityBase() { }

    protected EntityBase(TId id) => Id = id;
}