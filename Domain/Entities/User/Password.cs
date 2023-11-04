namespace Domain.Entities;

public sealed record Password
{
    private Password() { Value = null!; }
    public Password(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password), "Value is required");

        if (password.Length > 200)
            throw new ArgumentException("Value is too long", nameof(password));

        Value = password;
    }
    public string Value { get; }
}
