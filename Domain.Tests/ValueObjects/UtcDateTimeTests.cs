using Domain.ValueObjects;
using Xunit;

namespace Domain.Tests.ValueObjects;

public class UtcDateTimeTests
{
    [Fact]
    public void Create_ShouldSucceed_ForUtcDateTime()
    {
        var utcDateTime = DateTime.UtcNow;
        var vo = UtcDateTime.Create(utcDateTime);
        Assert.Equal(utcDateTime, vo.Value);
    }

    [Fact]
    public void Create_ShouldThrow_ForLocalDateTime()
    {
        var localDateTime = DateTime.Now;
        Assert.Throws<ArgumentException>(() => UtcDateTime.Create(localDateTime));
    }

    [Fact]
    public void Create_ShouldThrow_ForUnspecifiedDateTime()
    {
        var unspecifiedDateTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
        Assert.Throws<ArgumentException>(() => UtcDateTime.Create(unspecifiedDateTime));
    }
}