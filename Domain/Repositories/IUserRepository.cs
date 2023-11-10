using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories
{
    // User repository interface that extends the IGenericRepository interface
    public interface IUserRepository : IGenericRepository<User>
    {
        // Hash a password
        string HashPassword(string password);

        // Validate user credentials
        Task<User?> ValidateUserCredentialsAsync(string username, string password);

        // Send an email
        Task SendEmailAsync(string userEmail, string token);

        // Remove a user by their username
        Task<string> RemoveUserByUsernameAsync(string username);

        // Generate an email confirmation token
        string GenerateEmailConfirmationToken();

        // Fetch a user by their username
        Task<User?> FetchUserByUsernameAsync(string username, CancellationToken cancellationToken);
    }
}
