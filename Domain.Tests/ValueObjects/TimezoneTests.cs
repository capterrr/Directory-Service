using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class TimezoneTests
{
    [Theory]
    [InlineData("UTC")]
    [InlineData("Europe/Moscow")]
    [InlineData("America/New_York")]
    [InlineData(" A ")]
    public void Create_ShouldSucceed_ForValidTimezone(string validTimezone)
    {
        var timezone = Timezone.Create(validTimezone);
        Assert.Equal(validTimezone.Trim(), timezone.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_ShouldThrow_ForInvalidTimezone(string invalidTimezone)
    {
        Assert.Throws<ArgumentException>(() => Timezone.Create(invalidTimezone));
    }
}