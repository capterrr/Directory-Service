using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class IdentifierTests
{
    [Theory]
    [InlineData("test")]
    [InlineData("Test123")]
    [InlineData("test_123")]
    [InlineData("test-123")]
    [InlineData(" A ")]
    public void Create_ShouldSucceed_ForValidIdentifier(string validIdentifier)
    {
        var identifier = Identifier.Create(validIdentifier);
        Assert.Equal(validIdentifier.Trim(), identifier.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    [InlineData("тест")]
    [InlineData("test space")]
    [InlineData("test@")]
    [InlineData("test.123")]
    public void Create_ShouldThrow_ForInvalidIdentifier(string invalidIdentifier)
    {
        Assert.Throws<ArgumentException>(() => Identifier.Create(invalidIdentifier));
    }
}