using DCA_ASSIGNMENT.Core.Domain.Aggregates.Events;
using DCA_ASSIGNMENT.Core.Domain.Common.Values.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ViaEventAssociation.Infrastructure.EfcDmPersistence.VeaEventPersistence;

public class VeaEventEntityConfiguration : IEntityTypeConfiguration<ViaEvent>
{
    public void Configure(EntityTypeBuilder<ViaEvent> entity)
    {
        entity.ToTable("Events");

        entity.HasKey(e => e.Id);
        entity.Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                guid => EventId.Reconstitute(guid))
            .HasColumnName("Id");

        entity.Property<EventTitle>("Title")
            .HasConversion(
                t => t.Value,
                v => EventTitle.Reconstitute(v))
            .HasColumnName("Title")
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.Property<EventDescription>("Description")
            .HasConversion(
                d => d.Value,
                v => EventDescription.Reconstitute(v))
            .HasColumnName("Description")
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.Property<EventStatus>("Status")
            .HasConversion<string>()
            .HasColumnName("Status")
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.Property<EventMaxGuests>("MaxGuestNumber")
            .HasConversion(
                m => m.Value,
                v => EventMaxGuests.Reconstitute(v))
            .HasColumnName("MaxGuestNumber")
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.Property<EventVisibility>("EventVisibility")
            .HasConversion<string>()
            .HasColumnName("Visibility")
            .IsRequired()
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        entity.OwnsOne<EventTimes>("Times", timesNav =>
        {
            timesNav.Property(t => t.StartDate)
                .HasColumnName("StartDate");

            timesNav.Property(t => t.StartTime)
                .HasColumnName("StartTime");

            timesNav.Property(t => t.EndDate)
                .HasColumnName("EndDate");

            timesNav.Property(t => t.EndTime)
                .HasColumnName("EndTime");
        });
    }
}