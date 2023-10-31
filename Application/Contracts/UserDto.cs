using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts;

public record UserDto
{
    public Guid Id { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Email { get; init; }
    public List<Poste>? Postes { get; init; } = new();
    public Role Role { get; init; } = Role.User;
}
