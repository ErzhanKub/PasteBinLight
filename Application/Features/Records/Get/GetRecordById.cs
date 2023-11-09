using Domain.IServices;

namespace Application.Features.Records.Get
{
    // Request to get a record by its ID
    public record GetRecordByIdRequest : IRequest<Result<RecordDto>>
    {
        public Guid RecordId { get; init; }
        public Guid UserId { get; init; }
    }

    // Validator for the GetRecordByIdRequest
    public sealed class GetRecordByIdRequestValidator : AbstractValidator<GetRecordByIdRequest>
    {
        public GetRecordByIdRequestValidator()
        {
            RuleFor(p => p.RecordId).NotEmpty();
            RuleFor(p => p.UserId).NotEmpty();
        }
    }

    // Handler for the GetRecordByIdRequest
    public sealed class GetRecordByIdHandler : IRequestHandler<GetRecordByIdRequest, Result<RecordDto>>
    {
        private readonly IRecordRepository _recordRepository;
        private readonly IRecordCloudService _recordCloudService;
        private readonly ILogger<GetRecordByIdHandler> _logger;

        private const string RecordNotFoundMessage = "Record not found";
        private const string AccessDeniedMessage = "Access denied";
        private const string RecordReceivedMessage = "Record received: {id}";
        private const string ErrorMessage = "An error occurred while receiving the record";

        public GetRecordByIdHandler(IRecordRepository recordRepository, ILogger<GetRecordByIdHandler> logger, IRecordCloudService recordCloudService)
        {
            _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
        }

        // Handle the GetRecordByIdRequest
        public async Task<Result<RecordDto>> Handle(GetRecordByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var record = await _recordRepository.FetchByIdAsync(request.RecordId, cancellationToken).ConfigureAwait(false);

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
                    Title = record.Title,
                    Text = text,
                    DateCreated = record.DateCreated,
                    DeadLine = record.DeadLine,
                    IsPrivate = record.IsPrivate,
                    Likes = record.Likes,
                    DisLikes = record.DisLikes
                };

                _logger.LogInformation(RecordReceivedMessage, record.Id);
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
