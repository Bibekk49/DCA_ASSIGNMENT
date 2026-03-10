namespace DCA_ASSIGNMENT.Core.Domain.Common.Bases;

public abstract class IDEntity<TId>
{
    public TId Id { get; }

    protected IDEntity(TId id)
    {
        Id = id;
    }

}