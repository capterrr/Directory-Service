using Domain.Entities;
using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.Entities;

public class PositionTests
{
    private readonly Guid _id = Guid.NewGuid();
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private readonly DateTime _updatedAt = DateTime.UtcNow.AddMinutes(1);

    [Fact]
    public void Create_ShouldSucceed_ForValidData()
    {
        var position = Position.Create(
            EntityId.Create(_id),
            Name.Create("Test Position"),
            Description.Create("Test Desc"),
            IsActive.Create(true),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));

        Assert.Equal(_id, position.Id.Value);
        Assert.Equal("Test Position", position.Name.Value);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUpdatedAtBeforeCreatedAt()
    {
        Assert.Throws<ArgumentException>(() => Position.Create(
            EntityId.Create(_id),
            Name.Create("Test"),
            Description.Create("Desc"),
            IsActive.Create(true),
            UtcDateTime.Create(_updatedAt),
            UtcDateTime.Create(_createdAt)));
    }

    [Fact]
    public void UpdateName_ShouldSucceed_WhenNotArchived()
    {
        var position = CreateTestPosition();
        var newName = Name.Create("Updated Name");
        var newUpdatedAt = UtcDateTime.Create(_updatedAt.AddMinutes(1));

        var updated = position.UpdateName(newName, newUpdatedAt);

        Assert.Equal("Updated Name", updated.Name.Value);
        Assert.Equal(newUpdatedAt.Value, updated.UpdatedAt.Value);
    }

    [Fact]
    public void UpdateName_ShouldThrow_WhenArchived()
    {
        var position = CreateTestPosition().Archive(UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.Throws<InvalidOperationException>(() => position.UpdateName(Name.Create("New Name"), UtcDateTime.Create(_updatedAt.AddMinutes(2))));
    }

    [Fact]
    public void UpdateDescription_ShouldSucceed_WhenNotArchived()
    {
        var position = CreateTestPosition();
        var newDesc = Description.Create("Updated Desc");
        var newUpdatedAt = UtcDateTime.Create(_updatedAt.AddMinutes(1));

        var updated = position.UpdateDescription(newDesc, newUpdatedAt);

        Assert.Equal("Updated Desc", updated.Description.Value);
        Assert.Equal(newUpdatedAt.Value, updated.UpdatedAt.Value);
    }

    [Fact]
    public void Archive_ShouldSetIsActiveToFalse()
    {
        var position = CreateTestPosition();

        var archived = position.Archive(UtcDateTime.Create(_updatedAt.AddMinutes(1)));

        Assert.False(archived.IsActive.Value);
    }

    [Fact]
    public void LinkDepartment_ShouldSucceed_WhenNotLinked()
    {
        var position = CreateTestPosition();
        var deptId = EntityId.Create(Guid.NewGuid());

        var linked = position.LinkDepartment(deptId);

        Assert.Contains(deptId.Value, linked.DepartmentIds);
    }

    [Fact]
    public void LinkDepartment_ShouldThrow_WhenAlreadyLinked()
    {
        var position = CreateTestPosition();
        var deptId = EntityId.Create(Guid.NewGuid());
        position = position.LinkDepartment(deptId);

        Assert.Throws<InvalidOperationException>(() => position.LinkDepartment(deptId));
    }

    [Fact]
    public void UnlinkDepartment_ShouldSucceed_WhenLinked()
    {
        var position = CreateTestPosition();
        var deptId = EntityId.Create(Guid.NewGuid());
        position = position.LinkDepartment(deptId);

        var unlinked = position.UnlinkDepartment(deptId);

        Assert.DoesNotContain(deptId.Value, unlinked.DepartmentIds);
    }

    [Fact]
    public void UnlinkDepartment_ShouldThrow_WhenNotLinked()
    {
        var position = CreateTestPosition();
        var deptId = EntityId.Create(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => position.UnlinkDepartment(deptId));
    }

    [Fact]
    public void IsLinkedToDepartment_ShouldReturnTrue_WhenLinked()
    {
        var position = CreateTestPosition();
        var deptId = EntityId.Create(Guid.NewGuid());
        position = position.LinkDepartment(deptId);

        Assert.True(position.IsLinkedToDepartment(deptId));
    }

    [Fact]
    public void IsLinkedToDepartment_ShouldReturnFalse_WhenNotLinked()
    {
        var position = CreateTestPosition();
        var deptId = EntityId.Create(Guid.NewGuid());

        Assert.False(position.IsLinkedToDepartment(deptId));
    }

    private Position CreateTestPosition()
    {
        return Position.Create(
            EntityId.Create(_id),
            Name.Create("Test Position"),
            Description.Create("Test Desc"),
            IsActive.Create(true),
            UtcDateTime.Create(_createdAt),
            UtcDateTime.Create(_updatedAt));
    }
}