namespace Application.Features.Users.Create
{
    public record CreateUserDto
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
        public required string Email { get; init; }
    }
}
