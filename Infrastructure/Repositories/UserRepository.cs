using Domain.Entities;
using Domain.Repositories;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbcontext;

        public UserRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<User> CheckUserCredentialsAsync(string username, string password)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new ArgumentNullException("User not found");

            var hashedPassword = HashPassword(password);

            if (hashedPassword != user.Password)
                throw new ArgumentNullException("Incorrect password");

            return user;
        }

        public async Task<Guid> CreateAsync(User entity)
        {
            entity.Password = HashPassword(entity.Password);
            await _dbcontext.Users.AddAsync(entity);
            return entity.Id;
        }

        public Task<Guid[]> DeleteRangeAsync(params Guid[] ids)
        {
            var userToDelete = _dbcontext.Users.Where(u => ids.Contains(u.Id));
            _dbcontext.Users.RemoveRange(userToDelete);
            return Task.FromResult(ids);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbcontext.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                throw new ArgumentNullException("User not found");
            return user;
        }

        public string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hash;
        }

        public void Update(User entity)
        {
            _dbcontext.Users.Update(entity);
        }
    }
}