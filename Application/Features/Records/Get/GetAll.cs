namespace Application.Features.Records.Get;

public record GetAllRecordsRequest : IRequest<Result<List<GetAllRecordsDto>>> { }
public class GetAllRecordsRequestValidator : AbstractValidator<GetAllRecordsRequest>
{
    public GetAllRecordsRequestValidator() { }
}
public class GetAllRecordsHandler : IRequestHandler<GetAllRecordsRequest, Result<List<GetAllRecordsDto>>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly ILogger<GetAllRecordsHandler> _logger;

    private const string RecordReceivedMessega = "Received all public record";
    private const string ErrorMessega = "An error occurred while receiving mail";

    public GetAllRecordsHandler(IRecordRepository recordRepository, ILogger<GetAllRecordsHandler> logger)
    {
        _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllRecordsDto>>> Handle(GetAllRecordsRequest request, CancellationToken cancellationToken)
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
