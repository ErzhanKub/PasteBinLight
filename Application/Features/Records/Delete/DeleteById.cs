using Domain.IServices;

namespace Application.Features.Records.Delete
{
    // Command to delete a record by its ID
    public sealed record DeleteRecordByIdCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public Guid RecordId { get; init; }
    }

    // Validator for the DeleteRecordByIdCommand
    public sealed class DeleteRecordByIdCommandValidator : AbstractValidator<DeleteRecordByIdCommand>
    {
        public DeleteRecordByIdCommandValidator()
        {
            RuleFor(u => u.UserId)
                .NotEmpty()
                .NotNull();
            RuleFor(p => p.RecordId)
                .NotEmpty()
                .NotNull();
        }
    }

    // Handler for the DeleteRecordByIdCommand
    public sealed class DeleteRecordByIdHandler : IRequestHandler<DeleteRecordByIdCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecordRepository _recordRepository;
        private readonly IRecordCloudService _recordCloudService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteRecordByIdHandler> _logger;

        private const string UserNotFoundMessage = "User not found";
        private const string RecordNotFoundMessage = "Record not found";
        private const string RemovedRecordMessage = "Removed record from the cloud database: {id}";
        private const string ChangesSavedMessage = "Changes are saved to the local database";
        private const string ErrorMessage = "An error occurred while deleting mail";

        public DeleteRecordByIdHandler(IUserRepository userRepository, IRecordRepository recordRepository, IUnitOfWork unitOfWork, ILogger<DeleteRecordByIdHandler> logger, IRecordCloudService recordCloudService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
        }

        // Handle the DeleteRecordByIdCommand
        public async Task<Result<Guid>> Handle(DeleteRecordByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.FetchByIdAsync(request.UserId, cancellationToken);
                if (user is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail(UserNotFoundMessage);
                }

                var deletedRecord = user.Records!.FirstOrDefault(p => p.Id == request.RecordId);
                if (deletedRecord == null)
                {
                    _logger.LogWarning(RecordNotFoundMessage);
                    return Result.Fail<Guid>(RecordNotFoundMessage);
                }

                await _recordCloudService.DeleteTextFileFromCloudAsync(deletedRecord.Id.ToString());
                _logger.LogInformation(RemovedRecordMessage, deletedRecord.Id);

                var response = await _recordRepository.RemoveByIdAsync(deletedRecord.Id);
                _userRepository.Update(user);
                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                _logger.LogInformation(ChangesSavedMessage);

                return Result.Ok(response[0]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }
    }
}
