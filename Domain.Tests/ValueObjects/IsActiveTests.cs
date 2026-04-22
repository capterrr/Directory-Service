using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class IsActiveTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Create_ShouldSucceed_ForAnyBool(bool value)
    {
        var isActive = IsActive.Create(value);
        Assert.Equal(value, isActive.Value);
    }
}