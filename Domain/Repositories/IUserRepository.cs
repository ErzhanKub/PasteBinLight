using Domain.Entities;
using Domain.Shared;

namespace Domain.Repos
{
    public interface IUserRepository : IRepository<User>
    {
        string HashPassword(string password);
        Task<User> CheckUserCredentialsAsync(string username, string password);
    }
}