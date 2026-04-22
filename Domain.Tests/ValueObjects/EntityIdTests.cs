using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class EntityIdTests
{
    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789abc")]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    public void Create_ShouldSucceed_ForValidGuid(string validGuidString)
    {
        var validGuid = Guid.Parse(validGuidString);
        var entityId = EntityId.Create(validGuid);
        Assert.Equal(validGuid, entityId.Value);
    }

    [Fact]
    public void Create_ShouldThrow_ForEmptyGuid()
    {
        var emptyGuid = Guid.Empty;
        Assert.Throws<ArgumentException>(() => EntityId.Create(emptyGuid));
    }
}