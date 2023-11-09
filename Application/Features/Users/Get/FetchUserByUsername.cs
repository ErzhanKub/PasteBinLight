// Namespace for the user retrieval feature
namespace Application.Features.Users.Get
{
    // Class to handle the request for getting a user by username
    public record FetchUserByUsernameRequest : IRequest<Result<UserDto>>
    {
        public string? TargetUsername { get; init; }
    }

    // Validator class for the FetchUserByUsernameRequest
    public class FetchUserByUsernameValidator : AbstractValidator<FetchUserByUsernameRequest>
    {
        public FetchUserByUsernameValidator()
        {
            RuleFor(i => i.TargetUsername).NotEmpty();
        }
    }

    // Handler class for the FetchUserByUsernameRequest
    public class FetchUserByUsernameHandler : IRequestHandler<FetchUserByUsernameRequest, Result<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<FetchUserByUsernameHandler> _logger;

        private const string UserNotFoundMessage = "User not found";
        private const string UserRetrievedMessage = "Received user by: {Id}";
        private const string ErrorMessage = "An error occurred while receiving the user";

        public FetchUserByUsernameHandler(IUserRepository userRepo, ILogger<FetchUserByUsernameHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(FetchUserByUsernameRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.FetchUserByUsernameAsync(request.TargetUsername!,cancellationToken);

                if (user is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail<UserDto>(UserNotFoundMessage);
                }

                _logger.LogInformation(UserRetrievedMessage, user.Id);

                var response = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email.Value,
                    Username = request.TargetUsername,
                    Role = user.Role,
                };

                return Result.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }
    }
}
