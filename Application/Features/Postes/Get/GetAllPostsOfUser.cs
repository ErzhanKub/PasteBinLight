namespace Application.Features.Postes.Get;

public record GetAllPostsForUserRequest : IRequest<Result<List<GetAllPosteDto>>>
{
    public Guid UserId { get; init; }
}

public class GetAllPosteForUserRequestValidator : AbstractValidator<GetAllPostsForUserRequest>
{
    public GetAllPosteForUserRequestValidator()
    {
        RuleFor(u =>u.UserId).NotEmpty();
    }
}

public class GetAllPostsForUserHandler : IRequestHandler<GetAllPostsForUserRequest, Result<List<GetAllPosteDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllPostsForUserHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string ErrorMessega = "An error occurred while receiving poste from the user";
    private const string PostesRecieved = "All poste from the user have been received, userId: {Id}";

    public GetAllPostsForUserHandler(IUserRepository userRepository, ILogger<GetAllPostsForUserHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllPosteDto>>> Handle(GetAllPostsForUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var response = user.Postes.Select(poste => new GetAllPosteDto
            {
                Id = user.Id,
                DateCreated = poste.DateCreated,
                DisLikes = poste.DisLikes,
                Likes = poste.Likes,
                Title = poste.Title,
            }).ToList();

            _logger.LogInformation(PostesRecieved, user.Id);
            return Result.Ok(response);
        }
        catch (Exception  ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
