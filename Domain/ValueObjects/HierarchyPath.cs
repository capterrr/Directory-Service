namespace Domain.ValueObjects;

public sealed record HierarchyPath
{
    public string Value { get; }

    private HierarchyPath(string value)
    {
        Value = value;
    }

    public static HierarchyPath Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Path cannot be empty.", nameof(value));
        }

        return new HierarchyPath(value.Trim());
    }
}
