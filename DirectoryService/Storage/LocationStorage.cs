using Domain.Entities;
using Domain.ValueObjects;

namespace DirectoryService.Storage;

public static class LocationStorage
{
    private static readonly Dictionary<EntityId, Location> _locations = new();

    public static void Add(Location location)
    {
        if (_locations.ContainsKey(location.Id))
        {
            throw new InvalidOperationException($"Location with id '{location.Id.Value}' already exists.");
        }

        if (_locations.Values.Any(l => l.Name.Value == location.Name.Value && l.IsActive.Value))
        {
            throw new InvalidOperationException($"Location with name '{location.Name.Value}' already exists.");
        }

        _locations[location.Id] = location;
    }

    public static Location? GetById(EntityId id)
    {
        if (_locations.TryGetValue(id, out var location))
        {
            if (!location.IsActive.Value)
            {
                return null;
            }

            return location;
        }

        return null;
    }

    public static IEnumerable<Location> GetAll()
    {
        return _locations.Values.Where(l => l.IsActive.Value).ToList();
    }

    public static void Remove(EntityId id)
    {
        if (!_locations.TryGetValue(id, out var location))
        {
            throw new InvalidOperationException($"Location with id '{id.Value}' not found.");
        }

        if (!location.IsActive.Value)
        {
            throw new InvalidOperationException($"Location with id '{id.Value}' is already archived.");
        }

        var archivedLocation = location.Archive(UtcDateTime.Create(DateTime.UtcNow));
        _locations[id] = archivedLocation;
    }

    public static void HardRemove(EntityId id)
    {
        if (!_locations.ContainsKey(id))
        {
            throw new InvalidOperationException($"Location with id '{id.Value}' not found.");
        }

        _locations.Remove(id);
    }

    public static void UpdateLocation(Location updatedLocation)
    {
        if (!_locations.ContainsKey(updatedLocation.Id))
        {
            throw new InvalidOperationException($"Location with id '{updatedLocation.Id.Value}' not found.");
        }

        var existingLocation = _locations[updatedLocation.Id];
        
        if (!existingLocation.IsActive.Value)
        {
            throw new InvalidOperationException($"Location with id '{updatedLocation.Id.Value}' is archived.");
        }

        var locationWithSameName = _locations.Values.FirstOrDefault(l => 
            l.Name.Value == updatedLocation.Name.Value && 
            l.Id.Value != updatedLocation.Id.Value &&
            l.IsActive.Value);

        if (locationWithSameName != null)
        {
            throw new InvalidOperationException($"Location with name '{updatedLocation.Name.Value}' already exists.");
        }

        _locations[updatedLocation.Id] = updatedLocation;
    }

    public static void InitializeStorage()
    {
        if (_locations.Count > 0)
        {
            return;
        }

        try
        {
            // Location 1
            var location1 = Location.Create(
                EntityId.Create(new Guid("11111111-1111-1111-1111-111111111111")),
                Address.Create("123 Main St, Springfield, Illinois 62701"),
                Name.Create("Springfield Office"),
                Timezone.Create("America/Chicago"),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Add(location1);

            // Location 2
            var location2 = Location.Create(
                EntityId.Create(new Guid("22222222-2222-2222-2222-222222222222")),
                Address.Create("456 Oak Ave, New York, New York 10001"),
                Name.Create("New York Office"),
                Timezone.Create("America/New_York"),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Add(location2);

            // Location 3
            var location3 = Location.Create(
                EntityId.Create(new Guid("33333333-3333-3333-3333-333333333333")),
                Address.Create("789 Pine Road, Los Angeles, California 90001"),
                Name.Create("Los Angeles Office"),
                Timezone.Create("America/Los_Angeles"),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Add(location3);
        }
        catch
        {
            _locations.Clear();
            throw;
        }
    }
}
