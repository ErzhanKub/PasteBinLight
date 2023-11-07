using Domain.Enums;

namespace Application.Contracts;

public record UserDto
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public Role Role { get; init; } 
}
