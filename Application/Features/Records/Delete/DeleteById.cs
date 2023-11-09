using Domain.IServices;

namespace Application.Features.Records.Delete;

public record DeleteRecordByIdCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; set; }
    public Guid RecordId { get; init; }
}

public class DeleteRecordByIdsCommandValidator : AbstractValidator<DeleteRecordByIdCommand>
{
    public DeleteRecordByIdsCommandValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty()
            .NotNull();
        RuleFor(p => p.RecordId)
            .NotEmpty()
            .NotNull();
    }
}

public class DeleteRecordByIdsHandler : IRequestHandler<DeleteRecordByIdCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRecordRepository _recordRepository;
    private readonly IRecordCloudService _recordCloudService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRecordByIdsHandler> _logger;

    private const string UserNotFoundMessega = "User not found";
    private const string RecordNotFoundMessega = "Record not found";
    private const string RemovedTextMessega = "Removed record from the cloud database: {id}";
    private const string ChangesSavedMessega = "Changes are saved to the local database";
    private const string ErrorMessega = "An error occurred while deleting mail";

    public DeleteRecordByIdsHandler(IUserRepository userRepository, IRecordRepository recordRepository, IUnitOfWork unitOfWork, ILogger<DeleteRecordByIdsHandler> logger, IRecordCloudService recordCloudService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
    }

    public async Task<Result<Guid>> Handle(DeleteRecordByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var deletedRecord = user.Records!.FirstOrDefault(p => p.Id == request.RecordId);
            if (deletedRecord == null)
            {
                _logger.LogWarning(RecordNotFoundMessega);
                return Result.Fail<Guid>(RecordNotFoundMessega);
            }

            await _recordCloudService.DeleteTextFromCloudAsync(deletedRecord.Id.ToString());
            _logger.LogInformation(RemovedTextMessega, deletedRecord.Id);

            var response = await _recordRepository.DeleteByIdAsync(deletedRecord.Id);
            _userRepository.Update(user);
            await _unitOfWork.SaveCommitAsync();

            _logger.LogInformation(ChangesSavedMessega);

            return Result.Ok(response[0]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }

}
