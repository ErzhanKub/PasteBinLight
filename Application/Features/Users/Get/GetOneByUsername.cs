namespace Application.Features.Users.Get;

public record GetUserByUsernameRequest : IRequest<Result<UserDto>>
{
    public string? Username { get; init; }
}

public class GetUserByUsernameValidator : AbstractValidator<GetUserByUsernameRequest>
{
    public GetUserByUsernameValidator()
    {
        RuleFor(i => i.Username).NotEmpty();
    }
}

public class GetUserByUsernameRequestHandler : IRequestHandler<GetUserByUsernameRequest, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetOneUserRequestHandler> _logger;

    private const string UserNotFoundMessega = "User not found";
    private const string UserReceivedMessega = "Received user by: {Id}";
    private const string ErrorMessega = "An error occurred while receiving the user";

    public GetUserByUsernameRequestHandler(IUserRepository userRepository, ILogger<GetOneUserRequestHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<UserDto>> Handle(GetUserByUsernameRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username!);

            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail<UserDto>(UserNotFoundMessega);
            }

            _logger.LogInformation(UserReceivedMessega, user.Id);

            var response = new UserDto
            {
                Id = user.Id,
                Email = user.Email.Value,
                Username = request.Username,
                Role = user.Role,
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}