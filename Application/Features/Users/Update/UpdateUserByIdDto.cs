using Domain.Enums;

namespace Application.Features.Users.Update;

public record UpdateUserByIdDto
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }

    public required string Email { get; init; }
    public Role Role { get; init; }

}
