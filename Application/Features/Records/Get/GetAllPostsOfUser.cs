namespace Application.Features.Records.Get;

public record GetAllRecordsForUserRequest : IRequest<Result<List<GetAllRecordsDto>>>
{
    public Guid UserId { get; init; }
}

public class GetAllRecordsForUserRequestValidator : AbstractValidator<GetAllRecordsForUserRequest>
{
    public GetAllRecordsForUserRequestValidator()
    {
        RuleFor(u =>u.UserId).NotEmpty();
    }
}

public class GetAllPastsForUserHandler : IRequestHandler<GetAllRecordsForUserRequest, Result<List<GetAllRecordsDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetAllPastsForUserHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string ErrorMessega = "An error occurred while receiving record from the user";
    private const string RecordsRecieved = "All record from the user have been received, userId: {Id}";

    public GetAllPastsForUserHandler(IUserRepository userRepository, ILogger<GetAllPastsForUserHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllRecordsDto>>> Handle(GetAllRecordsForUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var response = user.Records.Select(record => new GetAllRecordsDto
            {
                Id = user.Id,
                DateCreated = record.DateCreated,
                DisLikes = record.DisLikes,
                Likes = record.Likes,
                Title = record.Title,
            }).ToList();

            _logger.LogInformation(RecordsRecieved, user.Id);
            return Result.Ok(response);
        }
        catch (Exception  ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
