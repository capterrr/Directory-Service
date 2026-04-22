namespace Domain.ValueObjects;

public sealed record Rank
{
    public short Value { get; }

    private Rank(short value)
    {
        Value = value;
    }

    public static Rank Create(short value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Rank must be positive.");
        }

        return new Rank(value);
    }
}