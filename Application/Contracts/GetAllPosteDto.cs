
namespace Application.Contracts
{
    public class GetAllPosteDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public DateTime DateCreated { get; set; }
        public long Likes { get; set; }
        public long DisLikes { get; set; }
    }
}
