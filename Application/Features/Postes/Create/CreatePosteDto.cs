namespace Application.Features.Postes.Create
{
    public record CreatePosteDto
    {
        public required string Text { get; init; }
        public string? Title { get; init; }
        public DateTime DeadLine { get; init; }
        public bool IsPrivate { get; init; }
    }
}
