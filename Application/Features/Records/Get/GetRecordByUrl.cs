using Domain.IServices;

namespace Application.Features.Records.Get
{
    // Request to get a record by its URL
    public record GetRecordByUrlRequest : IRequest<Result<RecordDto>>
    {
        public string? EncodedGuid { get; init; }
        public Guid UserId { get; init; }
    }

    // Validator for the GetRecordByUrlRequest
    public sealed class GetRecordByUrlRequestValidator : AbstractValidator<GetRecordByUrlRequest>
    {
        public GetRecordByUrlRequestValidator()
        {
            RuleFor(u => u.EncodedGuid).NotEmpty();
        }
    }

    // Handler for the GetRecordByUrlRequest
    public sealed class GetRecordByUrlHandler : IRequestHandler<GetRecordByUrlRequest, Result<RecordDto>>
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IRecordCloudService _recordCloudService;
        private readonly ILogger<GetRecordByUrlHandler> _logger;

        private const string RecordNotFoundMessage = "Record not found";
        private const string AccessDeniedMessage = "Access denied";
        private const string TextReceivedMessage = "Text received: {id}";
        private const string ErrorMessage = "An error occurred while receiving the text";

        public GetRecordByUrlHandler(IRecordRepository recordRepository, ILogger<GetRecordByUrlHandler> logger, IRecordCloudService recordCloudService)
        {
            _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
        }

        // Handle the GetRecordByUrlRequest
        public async Task<Result<RecordDto>> Handle(GetRecordByUrlRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var recordId = _recordRepository.DecodeGuidFromBase64(request.EncodedGuid!);

                var record = await _recordRepository.FetchByIdAsync(recordId, cancellationToken);

                if (record is null)
                {
                    _logger.LogWarning(RecordNotFoundMessage);
                    return Result.Fail(RecordNotFoundMessage);
                }

                if (record.IsPrivate && record.UserId != request.UserId)
                {
                    _logger.LogWarning(AccessDeniedMessage);
                    return Result.Fail<RecordDto>(AccessDeniedMessage);
                }

                var text = await _recordCloudService.FetchTextFileFromCloudAsync(record.Url);

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

                _logger.LogInformation(TextReceivedMessage, record.Id);

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
