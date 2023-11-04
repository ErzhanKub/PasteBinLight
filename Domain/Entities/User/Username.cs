namespace Domain.Entities;

public sealed record Username
{
    private Username() { Value = null!; }
    public Username(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name), "Value is required");

        if (name.Length > 200)
            throw new ArgumentException("Value is too long", nameof(name));

        Value = name;
    }
    public string Value { get; }
}