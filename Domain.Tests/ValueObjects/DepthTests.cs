using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class DepthTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void Create_ShouldSucceed_ForValidDepth(short validDepth)
    {
        var depth = Depth.Create(validDepth);
        Assert.Equal(validDepth, depth.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_ShouldThrow_ForInvalidDepth(short invalidDepth)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Depth.Create(invalidDepth));
    }
}