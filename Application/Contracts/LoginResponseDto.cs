namespace Application.Contracts
{
    public record LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
    }
}
