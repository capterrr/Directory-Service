using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class NameTests
{
    [Theory]
    [InlineData("Москва")]
    [InlineData("New York")]
    [InlineData("Test")]
    [InlineData(" A ")]
    public void Create_ShouldSucceed_ForValidName(string validName)
    {
        var name = Name.Create(validName);
        Assert.Equal(validName.Trim(), name.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_ShouldThrow_ForInvalidName(string invalidName)
    {
        Assert.Throws<ArgumentException>(() => Name.Create(invalidName));
    }
}