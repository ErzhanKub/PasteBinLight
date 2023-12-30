namespace Domain.Entities;

// Immutable record type for Username
public sealed record Username
{
    // Private constructor for EF Core
    private Username() { Value = null!; }

    // Public constructor with username validation
    public Username(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Username value is required");

        if (name.Length > 200)
            throw new ArgumentException("Username value is too long", nameof(name));

        Value = name;
    }

    // Username value
    public string Value { get; }
}
