// Importing the required libraries
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// Namespace for the user login feature
namespace Application.Features.Users.Login
{
    // Class to handle the request for logging in a user
    public record UserLoginRequest : IRequest<Result<LoginResponseDto>>
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
    }

    // Validator class for the UserLoginRequest
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(l => l.Username)
                .NotEmpty()
                .Length(1, 100)
                .Matches("^[a-zA-Z0-9]*$")
                .WithMessage("Username can only contain alphanumeric characters");

            RuleFor(l => l.Password)
                .NotEmpty()
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long")
                .Length(1, 16);
        }
    }

    // Handler class for the UserLoginRequest
    public class UserLoginHandler : IRequestHandler<UserLoginRequest, Result<LoginResponseDto>>
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserLoginHandler> _logger;

        private const string IncorrectInputMessage = "Username or password is incorrect";
        private const string TokenReceivedMessage = "Received a token for user by Id: {Id}; Token: {tokenString}";
        private const string ErrorMessage = "An error was received while receiving a token";

        public UserLoginHandler(IConfiguration config, IUserRepository userRepo, ILogger<UserLoginHandler> logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<LoginResponseDto>> Handle(UserLoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.ValidateUserCredentialsAsync(request.Username, request.Password);
                if (user is null)
                {
                    _logger.LogWarning(IncorrectInputMessage);
                    return Result.Fail(IncorrectInputMessage);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username.Value),
                    new Claim(ClaimTypes.Email, user.Email.Value),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var tokenString = GenerateTokenString(claims, DateTime.UtcNow.AddHours(1));

                _logger.LogInformation(TokenReceivedMessage, user.Id, tokenString);

                var response = new LoginResponseDto
                {
                    UserId = user.Id,
                    Token = tokenString,
                };

                return Result.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }

        private string GenerateTokenString(List<Claim> claims, DateTime exp)
        {
            var key = _config["Jwt"] ?? throw new Exception();
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
