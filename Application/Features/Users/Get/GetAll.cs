namespace Application.Features.Users.Get;

public record GetAllRequest : IRequest<IReadOnlyList<UserDto>> { }

public class GetAllRequestHandler : IRequestHandler<GetAllRequest, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllRequestHandler> _logger;

    private const string UserReceivedMessega = "Received all user";
    private const string ErrorMessega = "An error occurred while retrieving all users";

    public GetAllRequestHandler(IUserRepository userRepository, ILogger<GetAllRequestHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetAllRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var users = await _userRepository.GetAllAsync();

            _logger.LogInformation(UserReceivedMessega);

            var response = new List<UserDto>();

            foreach (var user in users)
            {
                var result = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username.Value,
                    Email = user.Email.Value,
                    Role = user.Role
                };
                response.Add(result);
            }
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
