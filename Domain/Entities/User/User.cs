﻿using Domain.Abstractions;
using Domain.Enums;
using System.Collections.ObjectModel;

namespace Domain.Entities
{
    public sealed class User : BaseEntity
    {
        private User():base(Guid.NewGuid()) { }
        private User(Guid id, Username username, Password password,
            Email email, Role role) : base(id)
        {
            Username = username;
            Password = password;
            Email = email;
            Role = role;
            Postes = new ReadOnlyCollection<Poste>(new List<Poste>());
        }

        public Username Username { get; private set; }
        public Password Password { get; private set; }
        public Email Email { get; private set; }
        public Role Role { get; private set; }

        public ICollection<Poste> Postes { get; private set; } = new List<Poste>();

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

        public void UpdateEmail(string newEmail)
        {
            Email = new Email(newEmail);
        }

        public void UpdateRole(Role role)
        {
            Role = role;
        }
    }

    public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
}