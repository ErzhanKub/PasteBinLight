namespace Application.Features.Records.Get
{
    // Request to get all public records
    public record GetAllPublicRecordsRequest : IRequest<Result<List<AllRecordsDto>>>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }

    // Validator for the GetAllPublicRecordsRequest
    public sealed class GetAllPublicRecordsRequestValidator : AbstractValidator<GetAllPublicRecordsRequest>
    {
        public GetAllPublicRecordsRequestValidator()
        {
            RuleFor(p => p.PageNumber).GreaterThan(0);
            RuleFor(p => p.PageSize).GreaterThan(0);
        }
    }

    // Handler for the GetAllPublicRecordsRequest
    public sealed class GetAllPublicRecordsHandler : IRequestHandler<GetAllPublicRecordsRequest, Result<List<AllRecordsDto>>>
    {
        private readonly IRecordRepository _recordRepository;
        private readonly ILogger<GetAllPublicRecordsHandler> _logger;

        private const string RecordReceivedMessage = "Received all public records";
        private const string ErrorMessage = "An error occurred while receiving records";

        public GetAllPublicRecordsHandler(IRecordRepository recordRepository, ILogger<GetAllPublicRecordsHandler> logger)
        {
            _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Handle the GetAllPublicRecordsRequest
        public async Task<Result<List<AllRecordsDto>>> Handle(GetAllPublicRecordsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var allRecords = await _recordRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);
                var publicRecords = allRecords.Where(p => p.IsPrivate != true).ToList();

                var response = publicRecords.Select(record => new AllRecordsDto
                {
                    Id = record.Id,
                    Url = record.Url,
                    Title = record.Title,
                    DateCreated = record.DateCreated,
                    DeadLine= record.DeadLine,
                    DisLikes = record.DisLikes,
                    Likes = record.Likes,
                }).ToList();

                _logger.LogInformation(RecordReceivedMessage);

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
