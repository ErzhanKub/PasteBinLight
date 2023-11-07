using Domain.IServices;

namespace Application.Features.Records.Get;

public record GetRecordByIdRequest : IRequest<Result<RecordDto>>
{
    public Guid RecordId { get; init; }
    public Guid UserId { get; init; }
}

public class GetRecordByIdValidator : AbstractValidator<GetRecordByIdRequest>
{
    public GetRecordByIdValidator()
    {
        RuleFor(p => p.RecordId).NotEmpty();
        RuleFor(p => p.UserId).NotEmpty();
    }
}

public sealed class GetRecordByIdHandler : IRequestHandler<GetRecordByIdRequest, Result<RecordDto>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly IRecordCloudService _recordCloudService;
    private readonly ILogger<GetRecordByIdHandler> _logger;

    private const string RecordNotFoundMessega = "Record not found";
    private const string AccessDeniedMessege = "Access denied";
    private const string RecordReceived = "Record received: {id}";
    private const string ErrorMessage = "An error occurred while receiving the record";
    public GetRecordByIdHandler(IRecordRepository recordRepository, IUserRepository userRepository, ILogger<GetRecordByIdHandler> logger, IRecordCloudService recordCloudService)
    {
        _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
    }

    public async Task<Result<RecordDto>> Handle(GetRecordByIdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var record = await _recordRepository.GetByIdAsync(request.RecordId).ConfigureAwait(false);
            if (record is null)
            {
                _logger.LogWarning(RecordNotFoundMessega);
                return Result.Fail(RecordNotFoundMessega);
            }


            if (record.IsPrivate && record.UserId != request.UserId)
            {
                _logger.LogWarning(AccessDeniedMessege);
                return Result.Fail<RecordDto>(AccessDeniedMessege);
            }

            var text = await _recordCloudService.GetTextFromCloudAsync(record.Url);

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

            _logger.LogInformation(RecordReceived, record.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}
