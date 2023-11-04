using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Features.Users.Login
{
    public record LoginRequest : IRequest<Result<string>>
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
    }
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(l => l.Username).NotEmpty().Length(1, 200);
            RuleFor(l => l.Password).NotEmpty().Length(1, 200);
        }
    }

    public class LoginHandler : IRequestHandler<LoginRequest, Result<string>>
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;

        public LoginHandler(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Result<string>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.CheckUserCredentialsAsync(request.Username, request.Password);
            if (user is null)
                return Result.Fail("Username or password is incorrect");

            var claims = new List<Claim>
            {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.Username.Value),
               new Claim(ClaimTypes.Email, user.Email.Value),
               new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var tokenString = GetTokenString(claims, DateTime.UtcNow.AddHours(1));

            return Result.Ok(tokenString);
        }

        private string GetTokenString(List<Claim> claims, DateTime exp)
        {
            var key = _configuration["Jwt"] ?? throw new Exception();
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: exp,
                signingCredentials: new SigningCredentials(
                    securityKey, SecurityAlgorithms.HmacSha256));

            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(token);
        }

    }
}
