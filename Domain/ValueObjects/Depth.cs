namespace Domain.ValueObjects;

public sealed record Depth
{
    public short Value { get; }

    private Depth(short value)
    {
        Value = value;
    }

    public static Depth Create(short value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Depth cannot be negative.");
        }

        return new Depth(value);
    }
}
