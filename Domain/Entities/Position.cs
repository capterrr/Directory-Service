using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Position
{
    public EntityId Id { get; }
    public Name Name { get; }
    public Description Description { get; }
    public IsActive IsActive { get; }
    public UtcDateTime CreatedAt { get; }
    public UtcDateTime UpdatedAt { get; }

    private readonly List<Guid> _departmentIds;

    public IReadOnlyCollection<Guid> DepartmentIds => _departmentIds.AsReadOnly();

    private Position(
        EntityId id,
        Name name,
        Description description,
        IsActive isActive,
        UtcDateTime createdAt,
        UtcDateTime updatedAt,
        IEnumerable<Guid>? departmentIds = null)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        _departmentIds = departmentIds is null ? new List<Guid>() : new List<Guid>(departmentIds);
    }

    public static Position Create(
        EntityId id,
        Name name,
        Description description,
        IsActive isActive,
        UtcDateTime createdAt,
        UtcDateTime updatedAt)
    {
        if (updatedAt.Value < createdAt.Value)
        {
            throw new ArgumentException("UpdatedAt cannot be earlier than CreatedAt.");
        }

        return new Position(id, name, description, isActive, createdAt, updatedAt);
    }

    public Position UpdateName(Name name, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        return new Position(Id, name, Description, IsActive, CreatedAt, updatedAt, _departmentIds);
    }

    public Position UpdateDescription(Description description, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        return new Position(Id, Name, description, IsActive, CreatedAt, updatedAt, _departmentIds);
    }

    public Position LinkDepartment(EntityId departmentId)
    {
        if (_departmentIds.Contains(departmentId.Value))
        {
            throw new InvalidOperationException("Position is already linked to this department.");
        }

        var updatedDepartmentIds = _departmentIds.Append(departmentId.Value).ToList();
        return new Position(Id, Name, Description, IsActive, CreatedAt, UpdatedAt, updatedDepartmentIds);
    }

    public Position UnlinkDepartment(EntityId departmentId)
    {
        if (!_departmentIds.Contains(departmentId.Value))
        {
            throw new InvalidOperationException("Position is not linked to this department.");
        }

        var updatedDepartmentIds = _departmentIds.Where(id => id != departmentId.Value).ToList();
        return new Position(Id, Name, Description, IsActive, CreatedAt, UpdatedAt, updatedDepartmentIds);
    }

    public bool IsLinkedToDepartment(EntityId departmentId)
    {
        return _departmentIds.Contains(departmentId.Value);
    }

    public Position Archive(UtcDateTime updatedAt)
    {
        EnsureUpdatedAtIsValid(updatedAt);

        return new Position(Id, Name, Description, IsActive.Create(false), CreatedAt, updatedAt, _departmentIds);
    }

    private void EnsureNotArchived()
    {
        if (!IsActive.Value)
        {
            throw new InvalidOperationException("Cannot modify archived position.");
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
