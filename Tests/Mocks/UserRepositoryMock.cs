//using Domain.Entities;
//using Domain.Repositories;
//using System.Security.Cryptography;
//using System.Text;

//namespace Tests.Mocks;

//internal class UserRepositoryMock : IUserRepository
//{
//    public Task<User?> CheckUserCredentialsAsync(string username, string password)
//    {
//        var user = User.Create(new Username(username), new Password(HashPassword(password)), new Email(username + "@gmail.com", true), Domain.Enums.Role.User, Guid.NewGuid().ToString());
//        return Task.FromResult(user ?? null);
//    }

//    public Task<Guid> CreateAsync(User entity)
//    {
//        throw new NotImplementedException();
//    }

//    public Task<Guid[]> DeleteRangeAsync(params Guid[] id)
//    {
//        throw new NotImplementedException();
//    }

//    public string GenerateEmailConfirmationToken()
//    {
//        throw new NotImplementedException();
//    }

//    public Task<IReadOnlyList<User>> GetAllAsync()
//    {
//        throw new NotImplementedException();
//    }

//    public Task<User?> GetByIdAsync(Guid id)
//    {
//        throw new NotImplementedException();
//    }

//    public string HashPassword(string password)
//    {
//        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
//        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
//        return hash;
//    }

//    public Task SendEmail(string userEmail, string token)
//    {
//        throw new NotImplementedException();
//    }

//    public void Update(User entity)
//    {
//        throw new NotImplementedException();
//    }
//}
