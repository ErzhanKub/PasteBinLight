namespace Application.Features.Postes.Get;

public record GetAllPastesForUserRequest : IRequest<Result<List<GetAllPasteDto>>>
{
    public Guid UserId { get; init; }
}

public class GetAllPasteForUserRequestValidator : AbstractValidator<GetAllPastesForUserRequest>
{
    public GetAllPasteForUserRequestValidator()
    {
        RuleFor(u =>u.UserId).NotEmpty();
    }
}

public class GetAllPastsForUserHandler : IRequestHandler<GetAllPastesForUserRequest, Result<List<GetAllPasteDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllPastsForUserHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string ErrorMessega = "An error occurred while receiving poste from the user";
    private const string PastesRecieved = "All poste from the user have been received, userId: {Id}";

    public GetAllPastsForUserHandler(IUserRepository userRepository, ILogger<GetAllPastsForUserHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllPasteDto>>> Handle(GetAllPastesForUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var response = user.Pastes.Select(paste => new GetAllPasteDto
            {
                Id = user.Id,
                DateCreated = paste.DateCreated,
                DisLikes = paste.DisLikes,
                Likes = paste.Likes,
                Title = paste.Title,
            }).ToList();

            _logger.LogInformation(PastesRecieved, user.Id);
            return Result.Ok(response);
        }
        catch (Exception  ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
