using Domain.Repositories;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbcontext;

        public UserRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<User?> CheckUserCredentialsAsync(string username, string password)
        {
            var hashedPassword = HashPassword(password);

            //Могут возникнуть проблемы с производительностью!
            var users = await _dbcontext.Users.ToListAsync();

            var user = users.FirstOrDefault(u => u.Username.Value == username && u.Password.Value == hashedPassword);

            return user;
        }


        public async Task<Guid> CreateAsync(User entity)
        {
            entity.UpdatePassword(HashPassword(entity.Password.Value));
            await _dbcontext.Users.AddAsync(entity);
            return entity.Id;
        }

        public Task<Guid[]> DeleteRangeAsync(params Guid[] ids)
        {
            var userToDelete = _dbcontext.Users.Where(u => ids.Contains(u.Id));
            _dbcontext.Users.RemoveRange(userToDelete);
            return Task.FromResult(ids);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _dbcontext.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == id);
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
