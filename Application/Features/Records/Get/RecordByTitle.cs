// Namespace for the record retrieval feature
namespace Application.Features.Records.Get
{
    // Class to handle the request for getting a record by title
    public record RecordByTitleRequest : IRequest<Result<List<AllRecordsDto>>>
    {
        // Title of the record to be retrieved
        public required string RecordTitle { get; init; }
    }

    // Validator class for the RecordByTitleRequest
    public sealed class RecordByTitleValidator : AbstractValidator<RecordByTitleRequest>
    {
        public RecordByTitleValidator()
        {
            // Rule to ensure the title is not empty
            RuleFor(x => x.RecordTitle).NotEmpty();
        }
    }

    // Handler class for the RecordByTitleRequest
    public sealed class RecordByTitleHandler : IRequestHandler<RecordByTitleRequest, Result<List<AllRecordsDto>>>
    {
        private readonly IRecordRepository _recordRepo;
        private readonly ILogger<RecordByTitleHandler> _logger;

        private const string NoRecordsFoundMessage = "Records not found";
        private const string RecordsRetrievedMessage = "Records received";
        private const string RetrievalErrorMessage = "An error was received when retrieving a record by title";

        public RecordByTitleHandler(IRecordRepository recordRepo, ILogger<RecordByTitleHandler> logger)
        {
            _recordRepo = recordRepo ?? throw new ArgumentNullException(nameof(recordRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<List<AllRecordsDto>>> Handle(RecordByTitleRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch records by title
                var records = await _recordRepo.FindRecordsByTitleAsync(request.RecordTitle, cancellationToken);
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
                _logger.LogError(RetrievalErrorMessage, ex);
                throw;
            }
        }
    }
}
