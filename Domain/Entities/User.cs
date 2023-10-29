using Domain.Enums;
namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public List<Poste>? Postes { get; set; } = new();
        public Role Role { get; set; } = Role.User;
    }
}