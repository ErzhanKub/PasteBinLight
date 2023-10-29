namespace Domain.Entities
{
    public class Poste
    {
        public Guid Id { get; set; }
        public required Uri Url { get; set; }
        public string? Title { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DeadLine { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
        public long Likes { get; set; }
        public long DisLikes { get; set; }
    }
}