using Domain.IServices;

namespace Application.Features.Records.Get;

public record GetOneRecordByUrlRequest : IRequest<Result<RecordDto>>
{
    public string? EncodedGuid { get; init; }
    public Guid UserId { get; init; }
}

public class GetOneRecordByUrlValidator : AbstractValidator<GetOneRecordByUrlRequest>
{
    public GetOneRecordByUrlValidator()
    {
        RuleFor(u => u.EncodedGuid).NotEmpty();
    }
}

public class GetOneRecordByUrlHandler : IRequestHandler<GetOneRecordByUrlRequest, Result<RecordDto>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly IRecordCloudService _recordCloudService;
    private readonly ILogger<GetOneRecordByUrlHandler> _logger;

    private const string RecordNotFoundMessega = "Record not found";
    private const string AccessDeniedMessega = "Access denied";
    private const string TextReceived = "Text received: {id}";
    private const string ErrorMessega = "An error occurred while receiving the text";

    public GetOneRecordByUrlHandler(IRecordRepository recordRepository, ILogger<GetOneRecordByUrlHandler> logger, IRecordCloudService recordCloudService)
    {
        _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
    }

    public async Task<Result<RecordDto>> Handle(GetOneRecordByUrlRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var recordId = _recordRepository.GetDecodedGuid(request.EncodedGuid!);

            var record = await _recordRepository.GetByIdAsync(recordId);

            if (record is null)
            {
                _logger.LogWarning(RecordNotFoundMessega);
                return Result.Fail(RecordNotFoundMessega);
            }

            if (record.IsPrivate && record.UserId != request.UserId)
            {
                _logger.LogWarning(AccessDeniedMessega);
                return Result.Fail<RecordDto>(AccessDeniedMessega);
            }


            var text = await _recordCloudService.GetTextFromCloudAsync(record.Url);

            var response = new RecordDto
            {
                Id = record.Id,
                DateCreated = record.DateCreated,
                DeadLine = record.DeadLine,
                DisLikes = record.DisLikes,
                IsPrivate = record.IsPrivate,
                Likes = record.Likes,
                Text = text ?? string.Empty,
                Title = record.Title,
            };

            _logger.LogInformation(TextReceived, record.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
