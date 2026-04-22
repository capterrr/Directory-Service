using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class AddressTests
{
    [Theory]
    [InlineData("Moscow, Red Square")]
    [InlineData("New York, 5th Ave")]
    [InlineData(" A ")]
    public void Create_ShouldSucceed_ForValidAddress(string validAddress)
    {
        var address = Address.Create(validAddress);
        Assert.Equal(validAddress.Trim(), address.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_ShouldThrow_ForInvalidAddress(string invalidAddress)
    {
        Assert.Throws<ArgumentException>(() => Address.Create(invalidAddress));
    }
}