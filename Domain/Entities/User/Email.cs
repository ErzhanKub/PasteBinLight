using System.Text.RegularExpressions;

namespace Domain.Entities;

// Immutable record type for Email
public sealed record Email
{
    // Regular expression for email validation
    private static readonly Regex EmailValidationRegex = new(@"^\w+([-+.']\w*)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled);

    // Private constructor for EF Core
    private Email() { Value = null!; }

    // Public constructor with email validation
    public Email(string? input, bool emailConfirmed)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentNullException(nameof(input), "Email value is required");

        string email = input.Trim();

        if (email.Length > 200)
            throw new ArgumentException("Email value is too long", nameof(Value));

        if (!EmailValidationRegex.IsMatch(email))
            throw new ArgumentException("Invalid email format", nameof(Value));

        Value = email;
        EmailConfirmed = emailConfirmed;
    }

    // Email value
    public string Value { get; }

    // Email confirmation status
    public bool EmailConfirmed { get; set; } = false;
}
