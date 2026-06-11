namespace ViaEventAssociation.Infrastructure.EfcQueries.Models;

public class Location
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int MaxCapacity { get; set; }

    public virtual ICollection<VeaEvent> Events { get; set; } = new List<VeaEvent>();
}
