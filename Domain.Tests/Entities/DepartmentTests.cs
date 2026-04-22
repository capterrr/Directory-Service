using Domain.Entities;
using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.Entities;

public class DepartmentTests
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly Guid _parentId = Guid.NewGuid();
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private readonly DateTime _updatedAt = DateTime.UtcNow.AddMinutes(1);

    [Fact]
    public void Create_ShouldSucceed_ForValidData()
    {
        var department = Department.Create(
            EntityId.Create(_id),
            Name.Create("Test Department"),
            Identifier.Create("test-dept"),
            ParentId.Create(_parentId),
            HierarchyPath.Create("root/test-dept"),
            Depth.Create(1),
            IsActive.Create(true),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));

        Assert.Equal(_id, department.Id.Value);
        Assert.Equal("Test Department", department.Name.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUpdatedAtBeforeCreatedAt()
    {
        Assert.Throws<ArgumentException>(() => Department.Create(
            EntityId.Create(_id),
            Name.Create("Test"),
            Identifier.Create("test"),
            ParentId.Create(null),
            HierarchyPath.Create("test"),
            Depth.Create(0),
            IsActive.Create(true),
            UtcDateTime.Create(_updatedAt),
            UtcDateTime.Create(_createdAt)));
    }

    [Fact]
    public void UpdateName_ShouldSucceed_WhenNotArchived()
    {
        var department = CreateTestDepartment();
        var newName = Name.Create("Updated Name");
        var newUpdatedAt = UtcDateTime.Create(_updatedAt.AddMinutes(1));

        var updated = department.UpdateName(newName, newUpdatedAt);

        Assert.Equal("Updated Name", updated.Name.Value);
        Assert.Equal(newUpdatedAt.Value, updated.UpdatedAt.Value);
    }

    [Fact]
    public void UpdateName_ShouldThrow_WhenArchived()
    {
        var department = CreateTestDepartment().Archive(UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => department.UpdateName(Name.Create("New Name"), UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void AddPosition_ShouldSucceed_WhenUnique()
    {
        var department = CreateTestDepartment();
        var position = CreateTestPosition();

        var updated = department.AddPosition(position, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Contains(updated.Positions, p => p.Id.Value == position.Id.Value);
        var addedPosition = updated.Positions.First(p => p.Id.Value == position.Id.Value);
        Assert.Contains(_id, addedPosition.DepartmentIds);
    }

    [Fact]
    public void AddPosition_ShouldThrow_WhenDuplicateId()
    {
        var department = CreateTestDepartment();
        var position = CreateTestPosition();
        department = department.AddPosition(position, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => department.AddPosition(position, UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void AddPosition_ShouldThrow_WhenDuplicateName()
    {
        var department = CreateTestDepartment();
        var position1 = CreateTestPosition();
        var position2 = Position.Create(
            EntityId.Create(Guid.NewGuid()),
            Name.Create("Test Position"),
            Description.Create("Desc"),
            IsActive.Create(true),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));

        department = department.AddPosition(position1, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => department.AddPosition(position2, UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void AddLocation_ShouldSucceed_WhenUnique()
    {
        var department = CreateTestDepartment();
        var location = CreateTestLocation();

        var updated = department.AddLocation(location, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Contains(updated.Locations, l => l.Id.Value == location.Id.Value);
        var addedLocation = updated.Locations.First(l => l.Id.Value == location.Id.Value);
        Assert.Contains(_id, addedLocation.DepartmentIds);
    }

    [Fact]
    public void AddLocation_ShouldThrow_WhenDuplicateId()
    {
        var department = CreateTestDepartment();
        var location = CreateTestLocation();
        department = department.AddLocation(location, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => department.AddLocation(location, UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void AddLocation_ShouldThrow_WhenDuplicateAddress()
    {
        var department = CreateTestDepartment();
        var location1 = CreateTestLocation();
        var location2 = Location.Create(
            EntityId.Create(Guid.NewGuid()),
            Address.Create("Test Address"),
            Name.Create("Loc2"),
            Timezone.Create("UTC"),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));

        department = department.AddLocation(location1, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => department.AddLocation(location2, UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void RemovePosition_ShouldSucceed_WhenExists()
    {
        var department = CreateTestDepartment();
        var position = CreateTestPosition();
        department = department.AddPosition(position, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        var updated = department.RemovePosition(position.Id, UtcDateTime.Create(_updatedAt.AddMinutes(2)));

        Assert.DoesNotContain(updated.Positions, p => p.Id.Value == position.Id.Value);
    }

    [Fact]
    public void RemovePosition_ShouldThrow_WhenNotExists()
    {
        var department = CreateTestDepartment();

        Assert.Throws<InvalidOperationException>(() => department.RemovePosition(EntityId.Create(Guid.NewGuid()), UtcDateTime.Create(_updatedAt.AddMinutes(1))));
    }

    [Fact]
    public void Archive_ShouldSetIsActiveToFalse()
    {
        var department = CreateTestDepartment();

        var archived = department.Archive(UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.False(archived.IsActive.Value);
    }

    [Fact]
    public void AttachToParent_ShouldSucceed_WhenValid()
    {
        var department = CreateTestDepartment();
        var parent = CreateTestDepartment(Guid.NewGuid(), "Parent", "parent");

        var attached = department.AttachToParent(parent, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Equal(parent.Id.Value, attached.ParentId.Value);
        Assert.Equal($"{parent.Path.Value}/{department.Identifier.Value}", attached.Path.Value);
        Assert.Equal(parent.Depth.Value + 1, attached.Depth.Value);
    }

    [Fact]
    public void AttachToParent_ShouldThrow_WhenSelf()
    {
        var department = CreateTestDepartment();

        Assert.Throws<InvalidOperationException>(() => department.AttachToParent(department, UtcDateTime.Create(_updatedAt.AddMinutes(1))));
    }

    [Fact]
    public void DetachFromParent_ShouldSucceed_WhenHasParent()
    {
        var department = CreateTestDepartment();
        var parent = CreateTestDepartment(Guid.NewGuid(), "Parent", "parent");
        department = department.AttachToParent(parent, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        var detached = department.DetachFromParent(UtcDateTime.Create(_updatedAt.AddMinutes(2)));

        Assert.Null(detached.ParentId.Value);
        Assert.Equal(department.Identifier.Value, detached.Path.Value);
        Assert.Equal(0, detached.Depth.Value);
    }

    [Fact]
    public void DetachFromParent_ShouldThrow_WhenNoParent()
    {
        var department = CreateTestDepartment();

        Assert.Throws<InvalidOperationException>(() => department.DetachFromParent(UtcDateTime.Create(_updatedAt.AddMinutes(1))));
    }

    [Fact]
    public void IsDescendantOf_ShouldReturnTrue_WhenDescendant()
    {
        var parent = CreateTestDepartment(Guid.NewGuid(), "Parent", "parent");
        var child = CreateTestDepartment();
        child = child.AttachToParent(parent, UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.True(child.IsDescendantOf(parent));
    }

    [Fact]
    public void IsDescendantOf_ShouldReturnFalse_WhenNotDescendant()
    {
        var dept1 = CreateTestDepartment();
        var dept2 = CreateTestDepartment();

        Assert.False(dept1.IsDescendantOf(dept2));
    }

    private Department CreateTestDepartment(Guid? customId = null, string name = "Test Department", string identifier = "test-dept")
    {
        var id = customId ?? _id;
        return Department.Create(
            EntityId.Create(id),
            Name.Create(name),
            Identifier.Create(identifier),
            ParentId.Create(null),
            HierarchyPath.Create(identifier),
            Depth.Create(0),
            IsActive.Create(true),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));
    }

    private Position CreateTestPosition()
    {
        return Position.Create(
            EntityId.Create(Guid.NewGuid()),
            Name.Create("Test Position"),
            Description.Create("Test Desc"),
            IsActive.Create(true),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));
    }

    private Location CreateTestLocation()
    {
        return Location.Create(
            EntityId.Create(Guid.NewGuid()),
            Address.Create("Test Address"),
            Name.Create("Test Location"),
            Timezone.Create("UTC"),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));
    }
}