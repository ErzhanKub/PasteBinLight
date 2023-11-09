namespace Application.Contracts;

public class AllRecordsDto
{
    public Guid Id { get; set; }  // Unique identifier
    public required Uri Url { get; set; }  // URL of the record
    public string? Title { get; set; }  // Title of the record
    public DateTime DateCreated { get; set; }  // Date when the record was created
    public DateTime DeadLine { get; set; }  // Deadline for the record
    public long Likes { get; set; }  // Number of likes
    public long DisLikes { get; set; }  // Number of dislikes
}
