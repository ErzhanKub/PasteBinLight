using Domain.Abstractions;
using Domain.Enums;
using System.Collections.ObjectModel;

namespace Domain.Entities;

public sealed class User : BaseEntity
{
    public User() : base(Guid.NewGuid()) { }
    public User(Guid id, Username username, Password password,
        Email email, Role role, string confirmationToken) : base(id)
    {
        Username = username;
        Password = password;
        Email = email;
        Role = role;
        Records = new ReadOnlyCollection<Record>(new List<Record>());
        ConfirmationToken = confirmationToken;
    }

        // Properties
        public Username Username { get; private set; }
        public Password Password { get; private set; }
        public Email Email { get; private set; }
        public Role Role { get; private set; }
        public string ConfirmationToken { get; private set; }
        public ICollection<Record> Records { get; private set; } = new List<Record>();

        // Factory method to create a new user
        public static User CreateUser(Username name, Password password,
            Email email, Role role, string confirmationToken)
        {
            var user = new User(Guid.NewGuid(), name, password, email, role, confirmationToken);
            user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
            return user;
        }

        // Add a record to the user's records
        public void AddRecordToUser(Record record)
        {
            var recordsList = Records.ToList();
            recordsList.Add(record);
            Records = new ReadOnlyCollection<Record>(recordsList);
        }

        // Remove a record from the user's records
        public void RemoveRecordFromUser(Record record)
        {
            var recordsList = Records.ToList();
            recordsList.Remove(record);
            Records = new ReadOnlyCollection<Record>(recordsList);
        }

        // Update the user's password
        public void UpdateUserPassword(string newPassword)
        {
            Password = new Password(newPassword);
        }

        // Update the user's username
        public void UpdateUserUsername(string newUsername)
        {
            Username = new Username(newUsername);
        }

        // Update the user's email
        public void UpdateUserEmail(string newEmail, bool emailConfirmed)
        {
            Email = new Email(newEmail, emailConfirmed);
        }

        // Update the user's role
        public void UpdateUserRole(Role role)
        {
            Role = role;
        }
    

    // Domain event for when a user is created
    public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
}

