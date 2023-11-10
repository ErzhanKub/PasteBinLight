using Application.Features.Records.Update;

namespace Application.Features.Records.Update;

public record ToLikeCommand : IRequest<Result<ForLikeDto>>
{
    public Guid UserId { get; init; }
    public ToLikeDto? Data { get; init; }
}
public record ToLikeDto 
{
    public Guid RecordId { get; init; }
    internal long Likes { get; set; }
}
public record ForLikeDto
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
public sealed class ToLikeValidator : AbstractValidator<ToLikeCommand>
{
    public ToLikeValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
    }
}

public sealed class ToLikeComamndHandler : IRequestHandler<ToLikeCommand, Result<ForLikeDto>>
{
    private readonly ILogger<ToLikeComamndHandler> _logger;
    private readonly IRecordRepository _recordRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToLikeComamndHandler(IRecordRepository recordRepository, IUnitOfWork unitOfWork, IUserRepository userRepository, ILogger<ToLikeComamndHandler> logger)
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

    public async Task<Result<ForLikeDto>> Handle(ToLikeCommand request, CancellationToken cancellationToken)
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

            record!.DisLikes = request.Data!.Likes + 1;

            _recordRepository.ToDislike(record);
            await _unitOfWork.SaveAndCommitAsync(cancellationToken);

            var response = new ForLikeDto
            {
                Id = record.Id,
                DeadLine = record.DeadLine,
                Title = record.Title,
                IsPrivate = record.IsPrivate,
                DateCreated = DateTime.Now,
                Likes = request.Data.Likes,
                DisLikes = record.DisLikes,
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


