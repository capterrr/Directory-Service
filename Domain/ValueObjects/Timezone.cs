namespace Domain.ValueObjects;

public sealed record Timezone
{
    public string Value { get; }

    private Timezone(string value)
    {
        Value = value;
    }

    public static Timezone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Timezone cannot be empty.", nameof(value));
        }

        return new Timezone(value.Trim());
    }
}
