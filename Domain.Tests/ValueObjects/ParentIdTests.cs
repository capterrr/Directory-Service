using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class ParentIdTests
{
    [Fact]
    public void Create_ShouldSucceed_ForNull()
    {
        var parentId = ParentId.Create(null);
        Assert.Null(parentId.Value);
    }

    [Theory]
    [InlineData("12345678-1234-1234-1234-123456789abc")]
    public void Create_ShouldSucceed_ForValidGuid(string validGuidString)
    {
        var validGuid = Guid.Parse(validGuidString);
        var parentId = ParentId.Create(validGuid);
        Assert.Equal(validGuid, parentId.Value);
    }

    [Fact]
    public void Create_ShouldThrow_ForEmptyGuid()
    {
        var emptyGuid = Guid.Empty;
        Assert.Throws<ArgumentException>(() => ParentId.Create(emptyGuid));
    }
}