namespace Domain.ValueObjects;

public sealed record ParentId
{
    public Guid? Value { get; }

    private ParentId(Guid? value)
    {
        Value = value;
    }

    public static ParentId Create(Guid? value)
    {
        if (value.HasValue && value.Value == Guid.Empty)
        {
            throw new ArgumentException("ParentId cannot be empty guid.", nameof(value));
        }

        return new ParentId(value);
    }
}
