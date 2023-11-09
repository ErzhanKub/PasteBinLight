using Domain.IServices;

namespace Application.Features.Records.Update;

public class UpdateRecordByIdCommand : IRequest<Result<RecordDto>>
{
    public Guid UserId { get; set; }
    public Guid RecordId { get; init; }
    public string? Title { get; init; }
    public string? Text { get; init; }
    public bool IsPrivate { get; init; }
    public DateTime DeadLine { get; init; }
}

public class UpdateRecordByIdValidator : AbstractValidator<UpdateRecordByIdCommand>
{
    public UpdateRecordByIdValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(p => p.RecordId).NotEmpty();
        RuleFor(p => p.Title).Length(1, 200);
        RuleFor(p => p.Text).Length(1, 4000);
        RuleFor(p => p.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
    }
}

public class UpdateRecordByIdHandler : IRequestHandler<UpdateRecordByIdCommand, Result<RecordDto>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRecordCloudService _recordCloudService;
    private readonly ILogger<UpdateRecordByIdHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string RecordNotFoundMessega = "Record is not found";
    private const string DataChangedMessage = "Data changed, Record: {id}";
    private const string ErrorMessega = "An error occurred while changing the text";

    public UpdateRecordByIdHandler(IRecordRepository repository, IUnitOfWork unitOfWork, IUserRepository userRepository, ILogger<UpdateRecordByIdHandler> logger, IRecordCloudService recordCloudService)
    {
        _recordRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
    }

    public async Task<Result<RecordDto>> Handle(UpdateRecordByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var record = user.Records.FirstOrDefault(p => p.Id == request.RecordId);

            if (record is null)
            {
                _logger.LogWarning(RecordNotFoundMessega);
                return Result.Fail(RecordNotFoundMessega);
            }

            if (request.Title is not null)
                record.Title = request.Title;

            record.DeadLine = request.DeadLine;
            record.IsPrivate = request.IsPrivate;

            await _recordCloudService.EditTextFromCloudeAsync(record.Id.ToString(), request.Text ?? string.Empty);

            _recordRepository.Update(record);
            await _unitOfWork.SaveCommitAsync();

            var response = new RecordDto
            {
                Id = record.Id,
                DeadLine = request.DeadLine,
                Text = request.Text ?? string.Empty,
                Title = request.Title,
                IsPrivate = request.IsPrivate,
            };

            _logger.LogInformation(DataChangedMessage, record.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
