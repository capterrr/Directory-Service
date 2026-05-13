using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class Location
{
    public EntityId Id { get; }
    public Address Address { get; }
    public Name Name { get; }
    public Timezone Timezone { get; }
    public IsActive IsActive { get; }
    public UtcDateTime CreatedAt { get; }
    public UtcDateTime UpdatedAt { get; }

    private readonly List<Guid> _departmentIds;

    public IReadOnlyCollection<Guid> DepartmentIds => _departmentIds.AsReadOnly();

    private Location(
        EntityId id,
        Address address,
        Name name,
        Timezone timezone,
        IsActive isActive,
        UtcDateTime createdAt,
        UtcDateTime updatedAt,
        IEnumerable<Guid>? departmentIds = null)
    {
        Id = id;
        Address = address;
        Name = name;
        Timezone = timezone;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        _departmentIds = departmentIds is null ? new List<Guid>() : new List<Guid>(departmentIds);
    }

    public static Location Create(
        EntityId id,
        Address address,
        Name name,
        Timezone timezone,
        UtcDateTime createdAt,
        UtcDateTime updatedAt,
        IsActive? isActive = null)
    {
        if (updatedAt.Value < createdAt.Value)
        {
            throw new ArgumentException("UpdatedAt cannot be earlier than CreatedAt.");
        }

        return new Location(
            id,
            address,
            name,
            timezone,
            isActive ?? IsActive.Create(true),
            createdAt,
            updatedAt);
    }

    public Location Update(Name name, Timezone timezone, Address address, UtcDateTime updatedAt)
    {
        EnsureNotArchived();
        EnsureUpdatedAtIsValid(updatedAt);

        return new Location(Id, address, name, timezone, IsActive, CreatedAt, updatedAt, _departmentIds);
    }

    public Location Archive(UtcDateTime updatedAt)
    {
        EnsureUpdatedAtIsValid(updatedAt);

        return new Location(Id, Address, Name, Timezone, IsActive.Create(false), CreatedAt, updatedAt, _departmentIds);
    }

    public Location LinkDepartment(EntityId departmentId)
    {
        if (_departmentIds.Contains(departmentId.Value))
        {
            throw new InvalidOperationException("Location is already linked to this department.");
        }

        var updatedDepartmentIds = _departmentIds.Append(departmentId.Value).ToList();
        return new Location(Id, Address, Name, Timezone, IsActive, CreatedAt, UpdatedAt, updatedDepartmentIds);
    }

    public Location UnlinkDepartment(EntityId departmentId)
    {
        if (!_departmentIds.Contains(departmentId.Value))
        {
            throw new InvalidOperationException("Location is not linked to this department.");
        }

        var updatedDepartmentIds = _departmentIds.Where(id => id != departmentId.Value).ToList();
        return new Location(Id, Address, Name, Timezone, IsActive, CreatedAt, UpdatedAt, updatedDepartmentIds);
    }

    public bool IsLinkedToDepartment(EntityId departmentId)
    {
        return _departmentIds.Contains(departmentId.Value);
    }

    private void EnsureNotArchived()
    {
        if (!IsActive.Value)
        {
            throw new InvalidOperationException("Cannot modify archived location.");
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
