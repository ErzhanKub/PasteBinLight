using Domain.Repositories;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Domain.Entities;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbcontext;
    private const string apiKey = "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";
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

    public async Task SendEmail(string userEmail, string token)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.brevo.com/v3/smtp/email"),
            Headers =
            {       
                { "accept", "application/json" },
                { "api-key", apiKey },
            },
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                sender = new { name = "ErzhanKub", email = "avazov.erjan@gmail.com" },
                to = new[] { new { email = userEmail } },
                subject = "Email confirmation",
                textContent = $"Thank you for registering! Please confirm your email by clicking on the following link: https://localhost:7056/api/User/{token}"
            }), Encoding.UTF8, "application/json")
        };

        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine(body);
        }
    }



    public void Update(User entity)
    {
        _dbcontext.Users.Update(entity);
    }

    public string GenerateEmailConfirmationToken()
    {
        byte[] tokenData = new byte[32];
        RandomNumberGenerator.Fill(tokenData);

        return Convert.ToBase64String(tokenData);
    }
}
