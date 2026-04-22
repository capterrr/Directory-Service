using Domain.Entities;
using Domain.ValueObjects;

namespace DirectoryService.Storage;

public static class PositionStorage
{
    private static readonly Dictionary<EntityId, Position> _positions = new();

    public static void Add(Position position)
    {
        if (_positions.ContainsKey(position.Id))
        {
            throw new InvalidOperationException($"Position with id '{position.Id.Value}' already exists.");
        }

        if (_positions.Values.Any(p => p.Name.Value == position.Name.Value && p.IsActive.Value))
        {
            throw new InvalidOperationException($"Position with name '{position.Name.Value}' already exists.");
        }

        _positions[position.Id] = position;
    }

    public static Position? GetById(EntityId id)
    {
        if (_positions.TryGetValue(id, out var position))
        {
            if (!position.IsActive.Value)
            {
                return null;
            }

            return position;
        }

        return null;
    }

    public static IEnumerable<Position> GetAll()
    {
        return _positions.Values.Where(p => p.IsActive.Value).ToList();
    }

    public static void Remove(EntityId id)
    {
        if (!_positions.TryGetValue(id, out var position))
        {
            throw new InvalidOperationException($"Position with id '{id.Value}' not found.");
        }

        if (!position.IsActive.Value)
        {
            throw new InvalidOperationException($"Position with id '{id.Value}' is already archived.");
        }

        var archivedPosition = position.Archive(UtcDateTime.Create(DateTime.UtcNow));
        _positions[id] = archivedPosition;
    }

    public static void HardRemove(EntityId id)
    {
        if (!_positions.ContainsKey(id))
        {
            throw new InvalidOperationException($"Position with id '{id.Value}' not found.");
        }

        _positions.Remove(id);
    }

    public static void UpdatePosition(Position updatedPosition)
    {
        if (!_positions.ContainsKey(updatedPosition.Id))
        {
            throw new InvalidOperationException($"Position with id '{updatedPosition.Id.Value}' not found.");
        }

        var existingPosition = _positions[updatedPosition.Id];
        
        if (!existingPosition.IsActive.Value)
        {
            throw new InvalidOperationException($"Position with id '{updatedPosition.Id.Value}' is archived.");
        }

        var positionWithSameName = _positions.Values.FirstOrDefault(p => 
            p.Name.Value == updatedPosition.Name.Value && 
            p.Id.Value != updatedPosition.Id.Value &&
            p.IsActive.Value);

        if (positionWithSameName != null)
        {
            throw new InvalidOperationException($"Position with name '{updatedPosition.Name.Value}' already exists.");
        }

        _positions[updatedPosition.Id] = updatedPosition;
    }

    public static void InitializeStorage()
    {
        if (_positions.Count > 0)
        {
            return;
        }

        try
        {
            // Position 1
            var position1 = Position.Create(
                EntityId.Create(new Guid("44444444-4444-4444-4444-444444444444")),
                Name.Create("Software Engineer"),
                Description.Create("Responsible for developing and maintaining software applications."),
                IsActive.Create(true),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Add(position1);

            // Position 2
            var position2 = Position.Create(
                EntityId.Create(new Guid("55555555-5555-5555-5555-555555555555")),
                Name.Create("Product Manager"),
                Description.Create("Responsible for defining product strategy and roadmap."),
                IsActive.Create(true),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Add(position2);

            // Position 3
            var position3 = Position.Create(
                EntityId.Create(new Guid("66666666-6666-6666-6666-666666666666")),
                Name.Create("DevOps Engineer"),
                Description.Create("Responsible for infrastructure and deployment automation."),
                IsActive.Create(true),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                UtcDateTime.Create(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)));

            Add(position3);
        }
        catch
        {
            _positions.Clear();
            throw;
        }
    }
}
