namespace Application.Features.Records.Update;

public record ToDislikeCommand : IRequest<Result<ForDisDto>>
{
    public Guid UserId { get; init; }
    public ToDislikeDto? Data { get; init; }
}
public record ToDislikeDto
{
    public Guid RecordId { get; init; }
    internal long DisLikes { get; set; }
}

public record ForDisDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool IsPrivate { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DeadLine { get; set; }
    public long Likes { get; set; }
    public long DisLikes { get; set; }
    public string? Text { get; set; }
}

public sealed class ToDislikeValidator : AbstractValidator<ToDislikeCommand>
{
    public ToDislikeValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
    }
}

public sealed class ToDislikeComamndHandler : IRequestHandler<ToDislikeCommand, Result<ForDisDto>>
{
    private readonly ILogger<ToDislikeComamndHandler> _logger;
    private readonly IRecordRepository _recordRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToDislikeComamndHandler(IRecordRepository recordRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, ILogger<ToDislikeComamndHandler> logger)
    {
        _recordRepository = recordRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _logger = logger;
    }

    private const string UserNotFoundMessage = "User is not found";
    private const string RecordNotFoundMessage = "Record is not found";
    private const string DataChangedMessage = "Data changed, Record: {id}";
    private const string ErrorMessage = "An error occurred while changing the text";

    public async Task<Result<ForDisDto>> Handle(ToDislikeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.FetchByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessage);
                return Result.Fail(UserNotFoundMessage);
            }

            var record = user?.Records.FirstOrDefault(p => p.Id == request.Data!.RecordId);
            if (record is null)
            {
                _logger.LogWarning(RecordNotFoundMessage);
                return Result.Fail(RecordNotFoundMessage);
            }

            record!.DisLikes = request.Data!.DisLikes + 1;

            _recordRepository.ToDislike(record);
            await _unitOfWork.SaveAndCommitAsync(cancellationToken);

            var response = new ForDisDto
            {
                Id = record.Id,
                DeadLine = record.DeadLine,
                Title = record.Title,
                IsPrivate = record.IsPrivate,
                DateCreated = DateTime.Now,
                Likes = record.Likes,
                DisLikes = request.Data.DisLikes,
            };
            _logger.LogInformation(DataChangedMessage, record.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
            {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}




