using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class HierarchyPathTests
{
    [Theory]
    [InlineData("root")]
    [InlineData("root/child")]
    [InlineData(" A ")]
    public void Create_ShouldSucceed_ForValidPath(string validPath)
    {
        var path = HierarchyPath.Create(validPath);
        Assert.Equal(validPath.Trim(), path.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_ShouldThrow_ForInvalidPath(string invalidPath)
    {
        Assert.Throws<ArgumentException>(() => HierarchyPath.Create(invalidPath));
    }
}