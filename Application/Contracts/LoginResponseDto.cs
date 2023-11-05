namespace Application.Contracts;

public record LoginResponseDto
{
    public Guid UserId { get; set; }
    public required string Token { get; set; }
}
