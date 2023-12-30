namespace Domain.Entities;

// Immutable record type for Password
public sealed record Password
{
    // Private constructor for EF Core
    private Password() { Value = null!; }

    // Public constructor with password validation
    public Password(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password), "Password value is required");

        if (password.Length > 200)
            throw new ArgumentException("Password value is too long", nameof(password));

        Value = password;
    }

    // Password value
    public string Value { get; }
}
