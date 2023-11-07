namespace Application.Contracts;

public record PasteDto
{
    public Guid Id { get; set; }
    public required string Text { get; set; }
    public string? Title { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DeadLine { get; set; }
    public long Likes { get; set; }
    public long DisLikes { get; set; }
}
