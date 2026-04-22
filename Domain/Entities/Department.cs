using System;
using System.Collections.Generic;
using System.Linq;
using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Department
{
    public EntityId Id { get; }
    public Name Name { get; }
    public Identifier Identifier { get; }
    public ParentId ParentId { get; }
    public HierarchyPath Path { get; }
    public Depth Depth { get; }
    public IsActive IsActive { get; }
    public UtcDateTime CreatedAt { get; }
    public UtcDateTime UpdatedAt { get; }

    private readonly List<PositionWithRank> _positions;
    private readonly List<Location> _locations;

    public IReadOnlyCollection<PositionWithRank> Positions => _positions.AsReadOnly();
    public IReadOnlyCollection<Location> Locations => _locations.AsReadOnly();

    private Department(
        EntityId id,
        Name name,
        Identifier identifier,
        ParentId parentId,
        HierarchyPath path,
        Depth depth,
        IsActive isActive,
        UtcDateTime createdAt,
        UtcDateTime updatedAt,
        IEnumerable<PositionWithRank>? positions,
        IEnumerable<Location>? locations)
    {
        Id = id;
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        _positions = positions is null ? new List<PositionWithRank>() : new List<PositionWithRank>(positions);
        _locations = locations is null ? new List<Location>() : new List<Location>(locations);
    }

    public static Department Create(
        EntityId id,
        Name name,
        Identifier identifier,
        ParentId parentId,
        HierarchyPath path,
        Depth depth,
        IsActive isActive,
        UtcDateTime createdAt,
        UtcDateTime updatedAt,
        IEnumerable<PositionWithRank>? positions = null,
        IEnumerable<Location>? locations = null)
    {
        if (updatedAt.Value < createdAt.Value)
        {
            throw new ArgumentException("UpdatedAt cannot be earlier than CreatedAt.");
        }

        return new Department(
            id,
            name,
            identifier,
            parentId,
            path,
            depth,
            isActive,
            createdAt,
            updatedAt,
            positions,
            locations);
    }

    public Department UpdateName(Name name, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        return new Department(
            Id,
            name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            _positions,
            _locations);
    }

    public Department AddPosition(Position position, Rank rank, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        if (_positions.Any(existing => existing.Position.Id.Value == position.Id.Value))
        {
            throw new InvalidOperationException("Position already added to department.");
        }

        if (_positions.Any(existing => string.Equals(existing.Position.Name.Value, position.Name.Value, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Position with the same name already exists in department.");
        }

        if (_positions.Any(existing => existing.Rank.Value == rank.Value))
        {
            throw new InvalidOperationException("Rank already assigned to another position in department.");
        }

        var linkedPosition = position.LinkDepartment(Id);
        var positionWithRank = new PositionWithRank(linkedPosition, rank);
        var updatedPositions = _positions.Append(positionWithRank).ToList();

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            updatedPositions,
            _locations);
    }

    public Department AddLocation(Location location, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        if (_locations.Any(existing => existing.Id.Value == location.Id.Value))
        {
            throw new InvalidOperationException("Location already added to department.");
        }

        if (_locations.Any(existing => string.Equals(existing.Address.Value, location.Address.Value, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Location with the same address already exists in department.");
        }

        var linkedLocation = location.LinkDepartment(Id);
        var updatedLocations = _locations.Append(linkedLocation).ToList();

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            _positions,
            updatedLocations);
    }

    public Department RemovePosition(EntityId positionId, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        var positionWithRank = _positions.FirstOrDefault(p => p.Position.Id.Value == positionId.Value);
        if (positionWithRank is null)
        {
            throw new InvalidOperationException("Position not found in department.");
        }

        var unlinkedPosition = positionWithRank.Position.UnlinkDepartment(Id);
        var updatedPositions = _positions.Where(p => p.Position.Id.Value != positionId.Value).ToList();

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            updatedPositions,
            _locations);
    }

    public Department RemoveLocation(EntityId locationId, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        if (!_locations.Any(l => l.Id.Value == locationId.Value))
        {
            throw new InvalidOperationException("Location not found in department.");
        }

        var locationToRemove = _locations.First(l => l.Id.Value == locationId.Value);
        var unlinkedLocation = locationToRemove.UnlinkDepartment(Id);
        var updatedLocations = _locations.Where(l => l.Id.Value != locationId.Value).ToList();

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            _positions,
            updatedLocations);
    }

    public Department UpdatePosition(EntityId positionId, Position updatedPosition, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        var index = _positions.FindIndex(p => p.Position.Id.Value == positionId.Value);
        if (index == -1)
        {
            throw new InvalidOperationException("Position not found in department.");
        }

        var currentRank = _positions[index].Rank;
        var linkedPosition = updatedPosition.LinkDepartment(Id);
        var updatedPositionWithRank = new PositionWithRank(linkedPosition, currentRank);
        var updatedPositions = new List<PositionWithRank>(_positions);
        updatedPositions[index] = updatedPositionWithRank;

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            updatedPositions,
            _locations);
    }

    public Department ChangePositionRank(EntityId positionId, Rank newRank, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        var index = _positions.FindIndex(p => p.Position.Id.Value == positionId.Value);
        if (index == -1)
        {
            throw new InvalidOperationException("Position not found in department.");
        }

        if (_positions.Any(existing => existing.Rank.Value == newRank.Value && existing.Position.Id.Value != positionId.Value))
        {
            throw new InvalidOperationException("Rank already assigned to another position in department.");
        }

        var currentPositionWithRank = _positions[index];
        var updatedPositionWithRank = currentPositionWithRank.WithRank(newRank);
        var updatedPositions = new List<PositionWithRank>(_positions);
        updatedPositions[index] = updatedPositionWithRank;

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive,
            CreatedAt,
            updatedAt,
            updatedPositions,
            _locations);
    }

    public Department MovePositionRank(EntityId positionId, short newRankValue, UtcDateTime updatedAt)
    {
        var newRank = Rank.Create(newRankValue);
        return ChangePositionRank(positionId, newRank, updatedAt);
    }

    public Department AttachToParent(Department parent, UtcDateTime updatedAt)
    {
        if (parent is null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        if (parent.Id.Value == Id.Value)
        {
            throw new InvalidOperationException("Department cannot join itself as a parent.");
        }

        if (ParentId.Value.HasValue && ParentId.Value == parent.Id.Value)
        {
            throw new InvalidOperationException("Department is already attached to this parent.");
        }

        var updatedPath = HierarchyPath.Create($"{parent.Path.Value}/{Identifier.Value}");
        var updatedDepth = Depth.Create((short)(parent.Depth.Value + 1));

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId.Create(parent.Id.Value),
            updatedPath,
            updatedDepth,
            IsActive,
            CreatedAt,
            updatedAt,
            _positions,
            _locations);
    }

    public Department DetachFromParent(UtcDateTime updatedAt)
    {
        if (!ParentId.Value.HasValue)
        {
            throw new InvalidOperationException("Department is already a root.");
        }

        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        var updatedPath = HierarchyPath.Create(Identifier.Value);
        var updatedDepth = Depth.Create(0);

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId.Create(null),
            updatedPath,
            updatedDepth,
            IsActive,
            CreatedAt,
            updatedAt,
            _positions,
            _locations);
    }

    public bool IsDescendantOf(Department potentialAncestor)
    {
        if (potentialAncestor is null)
        {
            throw new ArgumentNullException(nameof(potentialAncestor));
        }

        return Path.Value.StartsWith($"{potentialAncestor.Path.Value}/");
    }

    public bool IsAncestorOf(Department potentialDescendant)
    {
        if (potentialDescendant is null)
        {
            throw new ArgumentNullException(nameof(potentialDescendant));
        }

        return potentialDescendant.IsDescendantOf(this);
    }

    public Department Archive(UtcDateTime updatedAt)
    {
        EnsureUpdatedAtIsValid(updatedAt);

        return new Department(
            Id,
            Name,
            Identifier,
            ParentId,
            Path,
            Depth,
            IsActive.Create(false),
            CreatedAt,
            updatedAt,
            _positions,
            _locations);
    }

    private void EnsureNotArchived()
    {
        if (!IsActive.Value)
        {
            throw new InvalidOperationException("Cannot modify archived department.");
        }
    }

    private void EnsureUpdatedAtIsValid(UtcDateTime updatedAt)
    {
        if (updatedAt.Value <= UpdatedAt.Value)
        {
            throw new ArgumentException("UpdatedAt must be later than current UpdatedAt.", nameof(updatedAt));
        }
    }
}
