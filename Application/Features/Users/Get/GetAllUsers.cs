// Namespace for the user retrieval feature
namespace Application.Features.Users.Get
{
    // Class to handle the request for getting all users with pagination
    public record GetAllUsersRequest : IRequest<Result<List<UserDto>>>
    {
        public int PageSize { get; init; }
        public int PageNumber { get; init; }
    }

    // Validator class for the GetAllUsersRequest
    public class GetAllUsersValidator : AbstractValidator<GetAllUsersRequest>
    {
        public GetAllUsersValidator()
        {
            RuleFor(u => u.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0");
            RuleFor(u => u.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Page number must be 0 or greater");
        }
    }

    // Handler class for the GetAllUsersRequest
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersRequest, Result<List<UserDto>>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<GetAllUsersHandler> _logger;

        private const string UsersRetrievedMessage = "Received all users";
        private const string ErrorMessage = "An error occurred while retrieving all users";

        public GetAllUsersHandler(IUserRepository userRepo, ILogger<GetAllUsersHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<List<UserDto>>> Handle(GetAllUsersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepo.GetAllAsync(request.PageSize, request.PageNumber, cancellationToken);

                _logger.LogInformation(UsersRetrievedMessage);

                var response = users.Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username.Value,
                    Email = user.Email.Value,
                    Role = user.Role
                }).ToList();

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
