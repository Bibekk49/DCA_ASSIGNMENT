using System.Text.Json;
using ViaEventAssociation.Infrastructure.EfcQueries.Models;

namespace ViaEventAssociation.Infrastructure.EfcQueries.SeedFactories;

public static class SeedFactory
{
    public static async Task SeedAsync(ReadDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Events.Any()) return;

        var baseDir = FindDataDir();

        var events = LoadJson<EventJson>(Path.Combine(baseDir, "Events.json"));
        var locations = LoadJson<LocationJson>(Path.Combine(baseDir, "Locations.json"));

        context.Locations.AddRange(locations.Select(l => new Location
        {
            Id = l.Id,
            Name = l.Name,
            MaxCapacity = l.MaxCapacity
        }));

        context.Events.AddRange(events.Select(e =>
        {
            string? startDate = null, startTime = null, endDate = null, endTime = null;
            if (!string.IsNullOrEmpty(e.Start))
            {
                var parts = e.Start.Split(' ');
                startDate = parts[0];
                startTime = parts.Length > 1 ? parts[1] : null;
            }
            if (!string.IsNullOrEmpty(e.End))
            {
                var parts = e.End.Split(' ');
                endDate = parts[0];
                endTime = parts.Length > 1 ? parts[1] : null;
            }
            return new VeaEvent
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Status = e.Status,
                Visibility = e.Visibility,
                StartDate = startDate,
                StartTime = startTime,
                EndDate = endDate,
                EndTime = endTime,
                MaxGuestNumber = e.MaxGuests,
                LocationId = e.LocationId
            };
        }));

        await context.SaveChangesAsync();
    }

    private static List<T> LoadJson<T>(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? [];
    }

    private static string FindDataDir()
    {
        var dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            var candidate = Path.Combine(dir, "Context", "Assignment8", "ViaEventAssociation");
            if (Directory.Exists(candidate)) return candidate;
            dir = Path.GetDirectoryName(dir);
        }
        throw new DirectoryNotFoundException("Cannot find Context/Assignment8/ViaEventAssociation data folder");
    }

    private record EventJson(string Id, string Title, string Description, string Status, string Visibility, string? Start, string? End, int MaxGuests, string? LocationId);
    private record LocationJson(string Id, string Name, int MaxCapacity);
}
