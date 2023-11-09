// Namespace for the user retrieval feature
namespace Application.Features.Users.Get
{
    // Class to handle the request for getting a user by ID
    public record FetchUserByIdRequest : IRequest<Result<UserDto>>
    {
        public Guid UserId { get; init; }
    }

    // Validator class for the FetchUserByIdRequest
    public class FetchUserByIdValidator : AbstractValidator<FetchUserByIdRequest>
    {
        public FetchUserByIdValidator()
        {
            RuleFor(i => i.UserId).NotEmpty();
        }
    }

    // Handler class for the FetchUserByIdRequest
    public class FetchUserByIdHandler : IRequestHandler<FetchUserByIdRequest, Result<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<FetchUserByIdHandler> _logger;

        private const string UserNotFoundMessage = "User not found";
        private const string UserRetrievedMessage = "Received user by: {Id}";
        private const string ErrorMessage = "An error occurred while receiving the user";

        public FetchUserByIdHandler(IUserRepository userRepo, ILogger<FetchUserByIdHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(FetchUserByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.FetchByIdAsync(request.UserId, cancellationToken);

                if (user is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail<UserDto>(UserNotFoundMessage);
                }

                _logger.LogInformation(UserRetrievedMessage, user.Id);

                var response = new UserDto
                {
                    Id = request.UserId,
                    Email = user.Email.Value,
                    Username = user.Username.Value,
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
