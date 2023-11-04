using Domain.Enums;

namespace Application.Contracts;

public record UserDto
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public Role Role { get; init; } 
}
