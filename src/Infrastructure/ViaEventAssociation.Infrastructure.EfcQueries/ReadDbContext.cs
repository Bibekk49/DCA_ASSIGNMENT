using Microsoft.EntityFrameworkCore;
using ViaEventAssociation.Infrastructure.EfcQueries.Models;

namespace ViaEventAssociation.Infrastructure.EfcQueries;

public class ReadDbContext(DbContextOptions<ReadDbContext> options) : DbContext(options)
{
    public DbSet<VeaEvent> Events => Set<VeaEvent>();
    public DbSet<Location> Locations => Set<Location>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VeaEvent>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("Id");
            e.Property(x => x.Title).HasColumnName("Title");
            e.Property(x => x.Description).HasColumnName("Description");
            e.Property(x => x.Status).HasColumnName("Status");
            e.Property(x => x.Visibility).HasColumnName("Visibility");
            e.Property(x => x.StartDate).HasColumnName("StartDate");
            e.Property(x => x.StartTime).HasColumnName("StartTime");
            e.Property(x => x.EndDate).HasColumnName("EndDate");
            e.Property(x => x.EndTime).HasColumnName("EndTime");
            e.Property(x => x.MaxGuestNumber).HasColumnName("MaxGuestNumber");
            e.Property(x => x.LocationId).HasColumnName("LocationId");
            e.ToTable("Events");

            e.HasOne(x => x.Location)
             .WithMany(l => l.Events)
             .HasForeignKey(x => x.LocationId);
        });

        modelBuilder.Entity<Location>(e =>
        {
            e.HasKey(x => x.Id);
            e.ToTable("Locations");
        });
    }
}
