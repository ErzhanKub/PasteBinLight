using Domain.Repositories;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Cryptography;
using Domain.Entities;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbcontext;
    private const string apiKey = "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";
    public UserRepository(AppDbContext dbcontext)
    {
        _dbcontext = dbcontext;
    }

    // Check if user credentials are valid
    public async Task<User?> ValidateUserCredentialsAsync(string username, string password)
    {
        var hashedPassword = HashPassword(password);

        // Fetch users from database
        var users = await _dbcontext.Users.ToListAsync();

        // Find user with matching username and password
        var user = users.FirstOrDefault(u => u.Username.Value == username && u.Password.Value == hashedPassword);

        return user;
    }

    // Create a new user
    public async Task<Guid> CreateAsync(User entity, CancellationToken cancellationToken)
    {
        await _dbcontext.Users.AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    // Remove users by their IDs
    public Task<Guid[]> RemoveByIdAsync(params Guid[] ids)
    {
        var userToDelete = _dbcontext.Users.Where(u => ids.Contains(u.Id));
        _dbcontext.Users.RemoveRange(userToDelete);
        return Task.FromResult(ids);
    }

    // Fetch a user by their ID
    public async Task<User?> FetchByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return user;
    }

    // Hash a password
    public string HashPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hash;
    }

    // Send an email
    public async Task SendEmailAsync(string userEmail, string token)
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
                sender = new { name = "Erzhan Kubanchbek uulu", email = "avazov.erjan@gmail.com" },
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

    // Update a user
    public void Update(User entity)
    {
        _dbcontext.Users.Update(entity);
    }

    // Generate an email confirmation token
    public string GenerateEmailConfirmationToken()
    {
        byte[] tokenData = new byte[32];
        RandomNumberGenerator.Fill(tokenData);

        return Convert.ToBase64String(tokenData);
    }

    // Delete a user by their username
    public async Task<string> RemoveUserByUsernameAsync(string username)
    {
        var user = _dbcontext.Users.SingleOrDefault(u => u.Username != null && u.Username.Value == username);
        if (user != null)
        {
            _dbcontext.Users.Remove(user);
            await _dbcontext.SaveChangesAsync();
        }
        return username;
    }

    // Fetch a user by their username
    public async Task<User?> FetchUserByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var user = await _dbcontext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username.ToString() == username, cancellationToken);
        return user;
    }

    // Fetch all users with pagination
    public async Task<IReadOnlyList<User>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        return await _dbcontext.Users
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
