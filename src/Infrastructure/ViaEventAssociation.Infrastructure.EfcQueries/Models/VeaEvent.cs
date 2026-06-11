namespace ViaEventAssociation.Infrastructure.EfcQueries.Models;

public partial class VeaEvent
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public string Visibility { get; set; } = null!;
    public string? StartDate { get; set; }
    public string? StartTime { get; set; }
    public string? EndDate { get; set; }
    public string? EndTime { get; set; }
    public int MaxGuestNumber { get; set; }
    public string? LocationId { get; set; }

    public virtual Location? Location { get; set; }
}
