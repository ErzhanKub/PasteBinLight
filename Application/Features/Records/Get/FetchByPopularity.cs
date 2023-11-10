namespace Application.Features.Records.Get
{
    // Class to handle the request for getting records by popularity
    public record FetchByPopularityRequest : IRequest<Result<List<AllRecordsDto>>> { }

    // Validator class for the FetchByPopularityRequest
    public sealed class FetchByPopularityValidator : AbstractValidator<FetchByPopularityRequest>
    {
        public FetchByPopularityValidator()
        {
            // No validation rules required as there are no properties in the request
        }
    }

    // Handler class for the FetchByPopularityRequest
    public sealed class FetchByPopularityHandler : IRequestHandler<FetchByPopularityRequest, Result<List<AllRecordsDto>>>
    {
        private readonly IRecordRepository _recordRepo;
        private readonly ILogger<FetchByPopularityHandler> _logger;

        private const string NoRecordsFoundMessage = "Records not found";
        private const string RecordsRetrievedMessage = "Top 100 records retrieved";
        private const string RetrievalErrorMessage = "An error occurred while retrieving the top 100 records";

        public FetchByPopularityHandler(IRecordRepository recordRepo, ILogger<FetchByPopularityHandler> logger)
        {
            _recordRepo = recordRepo ?? throw new ArgumentNullException(nameof(recordRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<List<AllRecordsDto>>> Handle(FetchByPopularityRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch top 100 records by likes
                var records = await _recordRepo.FetchTop100RecordsByLikesAsync(cancellationToken);
                if (records is null)
                {
                    _logger.LogWarning(NoRecordsFoundMessage);
                    return Result.Fail(NoRecordsFoundMessage);
                }

                // Map the records to the DTO
                var response = records.Select(record => new AllRecordsDto
                {
                    Id = record.Id,
                    Title = record.Title,
                    Url = record.Url,
                    DateCreated = record.DateCreated,
                    DeadLine = record.DeadLine,
                    Likes = record.Likes,
                    DisLikes = record.DisLikes,
                }).ToList();

                _logger.LogInformation(RecordsRetrievedMessage);
                return Result.Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, RetrievalErrorMessage);
                throw;
            }
        }
    }
}
