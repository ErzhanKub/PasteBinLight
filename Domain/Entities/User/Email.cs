using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public sealed record Email
    {
        private static readonly Regex EmailRegex = new(@"^\w+([-+.']\w*)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", RegexOptions.Compiled);
        private Email() { Value = null!; }
        public Email(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input), "Value is required");

            string email = input.Trim();

            if (email.Length > 200)
                throw new ArgumentException("Value is too long", nameof(Value));

            if (!EmailRegex.IsMatch(email))
                throw new ArgumentException("Invalid email format", nameof(Value));

            Value = email;
        }

        public string Value { get; }
    }
}
