using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Features.Users.Login;
public record LoginRequest : IRequest<Result<LoginResponseDto>>
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
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

public class LoginHandler : IRequestHandler<LoginRequest, Result<LoginResponseDto>>
{
    private readonly IConfiguration _configuration;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(IConfiguration configuration, IUserRepository userRepository, ILogger<LoginHandler> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.CheckUserCredentialsAsync(request.Username, request.Password);
            if (user is null)
            {
                _logger.LogWarning("Username or password is incorrect");
                return Result.Fail("Username or password is incorrect");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username.Value),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var tokenString = GetTokenString(claims, DateTime.UtcNow.AddHours(1));

            _logger.LogInformation("Received a token for user by Id: {Id}; Token: {tokenString}", user.Id, tokenString);

            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Token = tokenString,
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error was received while receiving a token");
            throw;
        }
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
