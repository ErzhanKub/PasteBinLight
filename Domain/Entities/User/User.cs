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
        Records = new ReadOnlyCollection<Record>(new List<Record>());
        ConfirmationToken = confirmationToken;
    }

    public Username Username { get; private set; }
    public Password Password { get; private set; }
    public Email Email { get; private set; }
    public Role Role { get; private set; }
    public string ConfirmationToken { get; private set; }
    public ICollection<Record> Records { get; private set; } = new List<Record>();

    public static User Create(Username name, Password password,
        Email email, Role role, string confirmationToken)
    {
        var user = new User(Guid.NewGuid(), name, password, email, role, confirmationToken);
        user.Raise(new UserCreatedDomainEvent(user.Id));
        return user;
    }

    public void AddRecord(Record record)
    {
        var recordsList = Records.ToList();
        recordsList.Add(record);
        Records = new ReadOnlyCollection<Record>(recordsList);
    }

    public void RemoveRecord(Record record)
    {
        var recordsList = Records.ToList();
        recordsList.Remove(record);
        Records = new ReadOnlyCollection<Record>(recordsList);
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
