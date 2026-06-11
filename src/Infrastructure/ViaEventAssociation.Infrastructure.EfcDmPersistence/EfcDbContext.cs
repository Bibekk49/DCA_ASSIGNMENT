using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using Microsoft.EntityFrameworkCore;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence;

public class EfcDbContext(DbContextOptions<EfcDbContext> options) : DbContext(options)
{
    public DbSet<ViaEvent> Events => Set<ViaEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfcDbContext).Assembly);
}