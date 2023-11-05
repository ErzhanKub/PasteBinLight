namespace Application.Features.Users.Get;

public record GetOneUserRequest : IRequest<Result<UserDto>>
{
    public Guid Id { get; init; }
}

public class GetOneUserValidator : AbstractValidator<GetOneUserRequest>
{
    public GetOneUserValidator()
    {
        RuleFor(i => i.Id).NotEmpty();
    }
}

public class GetOneUserRequestHandler : IRequestHandler<GetOneUserRequest, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetOneUserRequestHandler> _logger;

    public GetOneUserRequestHandler(IUserRepository userRepository, ILogger<GetOneUserRequestHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<UserDto>> Handle(GetOneUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user is null)
            {
                _logger.LogWarning("User not found");
                return Result.Fail<UserDto>("User not found");
            }

            _logger.LogInformation("Received user by: {Id}", user.Id);

            var response = new UserDto
            {
                Id = request.Id,
                Email = user.Email.Value,
                Username = user.Username.Value,
                Role = user.Role,
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while receiving the user");
            throw;
        }
    }
}
