namespace Application.Features.Records.Get
{
    // Request to get all records for a user
    public record GetAllUserRecordsRequest : IRequest<Result<List<AllRecordsDto>>>
    {
        public Guid UserId { get; init; }
    }

    // Validator for the GetAllUserRecordsRequest
    public sealed class GetAllUserRecordsRequestValidator : AbstractValidator<GetAllUserRecordsRequest>
    {
        public GetAllUserRecordsRequestValidator()
        {
            RuleFor(u => u.UserId).NotEmpty();
        }
    }

    // Handler for the GetAllUserRecordsRequest
    public sealed class GetAllUserRecordsHandler : IRequestHandler<GetAllUserRecordsRequest, Result<List<AllRecordsDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetAllUserRecordsHandler> _logger;

        private const string UserNotFoundMessage = "User is not found";
        private const string ErrorMessage = "An error occurred while receiving records from the user";
        private const string RecordsReceivedMessage = "All records from the user have been received, userId: {Id}";

        public GetAllUserRecordsHandler(IUserRepository userRepository, ILogger<GetAllUserRecordsHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Handle the GetAllUserRecordsRequest
        public async Task<Result<List<AllRecordsDto>>> Handle(GetAllUserRecordsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.FetchByIdAsync(request.UserId, cancellationToken);
                if (user is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail(UserNotFoundMessage);
                }

                var response = user.Records.Select(record => new AllRecordsDto
                {
                    Id = record.Id,
                    Title = record.Title,
                    Url = record.Url,
                    DateCreated = record.DateCreated,
                    DeadLine = record.DeadLine,
                    DisLikes = record.DisLikes,
                    Likes = record.Likes,
                }).ToList();

                _logger.LogInformation(RecordsReceivedMessage, user.Id);
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
