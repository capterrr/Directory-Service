using Domain.Entities;
using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.Entities;

public class LocationTests
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private readonly DateTime _updatedAt = DateTime.UtcNow.AddMinutes(1);

    [Fact]
    public void Create_ShouldSucceed_ForValidData()
    {
        var location = Location.Create(
            EntityId.Create(_id),
            Address.Create("Test Address"),
            Name.Create("Test Location"),
            Timezone.Create("UTC"),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));

        Assert.Equal(_id, location.Id.Value);
        Assert.Equal("Test Address", location.Address.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUpdatedAtBeforeCreatedAt()
    {
        Assert.Throws<ArgumentException>(() => Location.Create(
            EntityId.Create(_id),
            Address.Create("Addr"),
            Name.Create("Loc"),
            Timezone.Create("UTC"),
            UtcDateTime.Create(_updatedAt),
            UtcDateTime.Create(_createdAt)));
    }

    [Fact]
    public void Update_ShouldSucceed_WhenNotArchived()
    {
        var location = CreateTestLocation();
        var newName = Name.Create("Updated Name");
        var newAddress = Address.Create("Updated Address");
        var newTimezone = Timezone.Create("Europe/Moscow");
        var newUpdatedAt = UtcDateTime.Create(_updatedAt.AddMinutes(1));

        var updated = location.Update(newName, newTimezone, newAddress, newUpdatedAt);

        Assert.Equal("Updated Name", updated.Name.Value);
        Assert.Equal("Updated Address", updated.Address.Value);
        Assert.Equal("Europe/Moscow", updated.Timezone.Value);
        Assert.Equal(newUpdatedAt.Value, updated.UpdatedAt.Value);
    }

    [Fact]
    public void Update_ShouldThrow_WhenArchived()
    {
        var location = CreateTestLocation().Archive(UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => location.Update(Name.Create("New"), Timezone.Create("UTC"), Address.Create("Addr"), UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void Archive_ShouldSetIsActiveToFalse()
    {
        var location = CreateTestLocation();

        var archived = location.Archive(UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.False(archived.IsActive.Value);
    }

    [Fact]
    public void LinkDepartment_ShouldSucceed_WhenNotLinked()
    {
        var location = CreateTestLocation();
        var deptId = EntityId.Create(Guid.NewGuid());

        var linked = location.LinkDepartment(deptId);

        Assert.Contains(deptId.Value, linked.DepartmentIds);
    }

    [Fact]
    public void LinkDepartment_ShouldThrow_WhenAlreadyLinked()
    {
        var location = CreateTestLocation();
        var deptId = EntityId.Create(Guid.NewGuid());
        location = location.LinkDepartment(deptId);

        Assert.Throws<InvalidOperationException>(() => location.LinkDepartment(deptId));
    }

    [Fact]
    public void UnlinkDepartment_ShouldSucceed_WhenLinked()
    {
        var location = CreateTestLocation();
        var deptId = EntityId.Create(Guid.NewGuid());
        location = location.LinkDepartment(deptId);

        var unlinked = location.UnlinkDepartment(deptId);

        Assert.DoesNotContain(deptId.Value, unlinked.DepartmentIds);
    }

    [Fact]
    public void UnlinkDepartment_ShouldThrow_WhenNotLinked()
    {
        var location = CreateTestLocation();
        var deptId = EntityId.Create(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => location.UnlinkDepartment(deptId));
    }

    [Fact]
    public void IsLinkedToDepartment_ShouldReturnTrue_WhenLinked()
    {
        var location = CreateTestLocation();
        var deptId = EntityId.Create(Guid.NewGuid());
        location = location.LinkDepartment(deptId);

        Assert.True(location.IsLinkedToDepartment(deptId));
    }

    [Fact]
    public void IsLinkedToDepartment_ShouldReturnFalse_WhenNotLinked()
    {
        var location = CreateTestLocation();
        var deptId = EntityId.Create(Guid.NewGuid());

        Assert.False(location.IsLinkedToDepartment(deptId));
    }

    private Location CreateTestLocation()
    {
        return Location.Create(
            EntityId.Create(_id),
            Address.Create("Test Address"),
            Name.Create("Test Location"),
            Timezone.Create("UTC"),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));
    }
}