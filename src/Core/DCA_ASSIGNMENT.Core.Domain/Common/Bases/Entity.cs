namespace DCA_ASSIGNMENT.Core.Domain.Common.Bases;

public abstract class Entity<TId>
{
    public TId Id { get; }

    protected Entity(TId id)
    {
        Id = id;
    }

}