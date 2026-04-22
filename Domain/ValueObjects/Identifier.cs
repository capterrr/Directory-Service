using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed record Identifier
{
    private static readonly Regex LatinIdentifierRegex = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

    public string Value { get; }

    private Identifier(string value)
    {
        Value = value;
    }

    public static Identifier Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Identifier cannot be empty.", nameof(value));
        }

        var normalized = value.Trim();
        if (!LatinIdentifierRegex.IsMatch(normalized))
        {
            throw new ArgumentException("Identifier should contain only latin letters, digits, '_' or '-'.", nameof(value));
        }

        return new Identifier(normalized);
    }
}
