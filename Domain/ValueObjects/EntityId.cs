namespace Domain.ValueObjects;

public sealed record EntityId
{
    public Guid Value { get; }

    private EntityId(Guid value)
    {
        Value = value;
    }

    public static EntityId Create()
    {
        return new EntityId(Guid.NewGuid());
    }

    public static EntityId Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.", nameof(value));
        }

        return new EntityId(value);
    }
}
