using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class DescriptionTests
{
    [Theory]
    [InlineData("Some description")]
    [InlineData("Another one")]
    [InlineData(" A ")]
    public void Create_ShouldSucceed_ForValidDescription(string validDescription)
    {
        var description = Description.Create(validDescription);
        Assert.Equal(validDescription.Trim(), description.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_ShouldThrow_ForInvalidDescription(string invalidDescription)
    {
        Assert.Throws<ArgumentException>(() => Description.Create(invalidDescription));
    }
}