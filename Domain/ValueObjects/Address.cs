namespace Domain.ValueObjects;

public sealed record Address
{
    public string Value { get; }

    private Address(string value)
    {
        Value = value;
    }

    public static Address Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Address cannot be empty.", nameof(value));
        }

        return new Address(value.Trim());
    }
}
