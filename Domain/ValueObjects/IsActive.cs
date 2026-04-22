namespace Domain.ValueObjects;

public sealed record IsActive
{
    public bool Value { get; }

    private IsActive(bool value)
    {
        Value = value;
    }

    public static IsActive Create(bool value)
    {
        return new IsActive(value);
    }
}
