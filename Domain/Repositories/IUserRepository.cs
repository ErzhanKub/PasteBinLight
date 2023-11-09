﻿using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    string HashPassword(string password);
    Task<User?> CheckUserCredentialsAsync(string username, string password);
    Task SendEmail(string userEmail, string token);
    Task<string> DeleteUserByUsernameAsync(string username);
    string GenerateEmailConfirmationToken();
    Task<User?> GetByUsernameAsync(string username);
}