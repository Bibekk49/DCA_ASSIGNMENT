namespace DCA_ASSIGNMENT.Core.Domain.Common;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}
