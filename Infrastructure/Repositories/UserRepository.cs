using Domain.Entities;
using Domain.Repositories;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public Task<User> CheckUserCredentialsAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<Guid[]> DeleteRangeAsync(params Guid[] id)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public string HashPassword(string password)
        {
            throw new NotImplementedException();
        }

        public void Update(User entity)
        {
            throw new NotImplementedException();
        }
    }
}