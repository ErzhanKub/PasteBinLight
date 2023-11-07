using Domain.Abstractions;
using Domain.Enums;
using System.Collections.ObjectModel;

namespace Domain.Entities;

public sealed class User : BaseEntity
{
    private User() : base(Guid.NewGuid()) { }
    private User(Guid id, Username username, Password password,
        Email email, Role role, string confirmationToken) : base(id)
    {
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        Pastes = new ReadOnlyCollection<Paste>(new List<Paste>());
        ConfirmationToken = confirmationToken;
    }

    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public Email Email { get; private set; }
    public Role Role { get; private set; }
    public string ConfirmationToken { get; private set; }
    public ICollection<Paste> Pastes { get; private set; } = new List<Paste>();

    public static User Create(Username name, Password password,
        Email email, Role role, string confirmationToken)
    {
        var user = new User(Guid.NewGuid(), name, password, email, role, confirmationToken);
        user.Raise(new UserCreatedDomainEvent(user.Id));
        return user;
    }

    public void AddPaste(Paste poste)
    {
        var postesList = Pastes.ToList();
        postesList.Add(poste);
        Pastes = new ReadOnlyCollection<Paste>(postesList);
    }

    public void RemovePoste(Paste poste)
    {
        var postesList = Pastes.ToList();
        postesList.Remove(poste);
        Pastes = new ReadOnlyCollection<Paste>(postesList);
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
