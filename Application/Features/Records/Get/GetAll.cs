namespace Application.Features.Records.Get;

public record GetAllPublicRecordsRequest : IRequest<Result<List<GetAllRecordsDto>>> { }
public class GetAllPublicRecordsRequestValidator : AbstractValidator<GetAllPublicRecordsRequest>
{
    public GetAllPublicRecordsRequestValidator() { }
}
public class GetAllPublicRecordsHandler : IRequestHandler<GetAllPublicRecordsRequest, Result<List<GetAllRecordsDto>>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly ILogger<GetAllPublicRecordsHandler> _logger;

    private const string RecordReceivedMessega = "Received all public record";
    private const string ErrorMessega = "An error occurred while receiving mail";

    public GetAllPublicRecordsHandler(IRecordRepository recordRepository, ILogger<GetAllPublicRecordsHandler> logger)
    {
        _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllRecordsDto>>> Handle(GetAllPublicRecordsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var allRecords = await _recordRepository.GetAllAsync();
            var publicPostes = allRecords.Where(p => p.IsPrivate != true).ToList();

            var response = publicPostes.Select(record => new GetAllRecordsDto
            {
                Id = record.Id,
                DateCreated = record.DateCreated,
                DisLikes = record.DisLikes,
                Likes = record.Likes,
                Title = record.Title,
            }).ToList();

            _logger.LogInformation(RecordReceivedMessega);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
