using Domain.IServices;

namespace Application.Features.Records.Create;

public record CreateRecordCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public required string Text { get; init; }
    public string? Title { get; init; }
    public DateTime DeadLine { get; init; }
    public bool IsPrivate { get; init; }
}

public class CreateRecordCommandValidator : AbstractValidator<CreateRecordCommand>
{
    public CreateRecordCommandValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(t => t.Text).NotEmpty().Length(1, 4000);
        RuleFor(t => t.Title).Length(1, 200);
        RuleFor(d => d.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
    }
}

public sealed class CreateRecordHandler : IRequestHandler<CreateRecordCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRecordRepository _recordRepository;
    private readonly IRecordCloudService _recordCloudService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRecordHandler> _logger;
    
    private const string UserNotFoundMessage = "User not found";
    private const string LoadingTextMessage = "Loading text into the cloud database: {Id}";
    private const string AddedRecordMessage = "Added record to user";
    private const string RecordSavedMessage = "Record saved to local database, recordId: {recordId}; userId: {userId}";
    private const string ErrorMessage = "An error occurred during record creation";

    public CreateRecordHandler(IUserRepository userRepository, IRecordRepository recordRepository, IUnitOfWork unitOfWork, ILogger<CreateRecordHandler> logger, IRecordCloudService recordCloudService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
    }

    public async Task<Result<string>> Handle(CreateRecordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessage);
                return Result.Fail(UserNotFoundMessage);
            }

            var recordId = Guid.NewGuid();

            var urlCloud = await _recordCloudService.UploadTextToCloudAsync(recordId.ToString(), request.Text);

            _logger.LogInformation(LoadingTextMessage, recordId);

            var record = new Record
            {
                Id = recordId,
                DateCreated = DateTime.Now,
                DeadLine = request.DeadLine,
                IsPrivate = request.IsPrivate,
                Title = request.Title,
                Url = new Uri(urlCloud),
                User = user,
                UserId = user.Id,
            };

            user.AddRecord(record);
            _logger.LogInformation(AddedRecordMessage);

            var guid = await _recordRepository.CreateAsync(record);

            _userRepository.Update(user);

            await _unitOfWork.SaveCommitAsync();
            _logger.LogInformation(RecordSavedMessage, record.Id, user.Id);

            var encodedGuid = _recordRepository.GetEncodedGuid(guid);

            return Result.Ok(encodedGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}
