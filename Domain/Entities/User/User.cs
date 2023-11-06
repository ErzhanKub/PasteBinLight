using Domain.Abstractions;
using Domain.Enums;
using System.Collections.ObjectModel;

namespace Domain.Entities
{
    public sealed class User : BaseEntity
    {
        public User():base(Guid.NewGuid()) { }
        public User(Guid id, Username username, Password password,
            Email email, Role role) : base(id)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            Postes = new ReadOnlyCollection<Poste>(new List<Poste>());
        }

        public Username Username { get; set; }
        public Password Password { get; set; }
        public Email Email { get; set; }
        public Role Role { get; set; }

        public ICollection<Poste> Postes { get; set; } = new List<Poste>();

        public static User Create(Username name, Password password,
            Email email, Role role)
        {
            var user = new User(Guid.NewGuid(), name, password, email, role);
            user.Raise(new UserCreatedDomainEvent(user.Id));
            return user;
        }

        public void AddPoste(Poste poste)
        {
            var postesList = Postes.ToList();
            postesList.Add(poste);
            Postes = new ReadOnlyCollection<Poste>(postesList);
        }

        public void RemovePoste(Poste poste)
        {
            var postesList = Postes.ToList();
            postesList.Remove(poste);
            Postes = new ReadOnlyCollection<Poste>(postesList);
        }

        public void UpdatePassword(string newPassword)
        {
            Password = new Password(newPassword);
        }
        public void UpdateUsername(string newUsername)
        {
            Username = new Username(newUsername);
        }

        public void UpdateEmail(string newEmail, bool emailConfirmed)
        {
            Email = new Email(newEmail, emailConfirmed);
        }

        public void UpdateRole(Role role)
        {
            Role = role;
        }
    }

    public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
}
