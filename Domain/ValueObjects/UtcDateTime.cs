namespace Domain.ValueObjects;

public sealed record UtcDateTime
{
    public DateTime Value { get; }

    private UtcDateTime(DateTime value)
    {
        Value = value;
    }

    public static UtcDateTime Create(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("DateTime value must be in UTC.", nameof(value));
        }

        return new UtcDateTime(value);
    }
}
