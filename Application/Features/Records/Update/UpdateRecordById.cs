using Domain.IServices;

namespace Application.Features.Records.Update
{
    // Class to handle the command for updating a record by ID
    public record UpdateRecordByIdCommand : IRequest<Result<RecordDto>>
    {
        public Guid UserId { get; set; }
        public Guid RecordId { get; init; }
        public string? NewTitle { get; init; }
        public string? NewText { get; init; }
        public bool NewPrivacyStatus { get; init; }
        public DateTime NewDeadLine { get; init; }
    }

    // Validator class for the UpdateRecordByIdCommand
    public sealed class UpdateRecordByIdValidator : AbstractValidator<UpdateRecordByIdCommand>
    {
        public UpdateRecordByIdValidator()
        {
            RuleFor(u => u.UserId).NotEmpty();
            RuleFor(p => p.RecordId).NotEmpty();
            RuleFor(p => p.NewTitle).Length(1, 200);
            RuleFor(p => p.NewText).Length(1, 4000);
            RuleFor(p => p.NewDeadLine).GreaterThanOrEqualTo(DateTime.Now);
        }
    }

    // Handler class for the UpdateRecordByIdCommand
    public sealed class UpdateRecordByIdHandler : IRequestHandler<UpdateRecordByIdCommand, Result<RecordDto>>
    {
        private readonly IRecordRepository _recordRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly IRecordCloudService _recordCloudService;
        private readonly ILogger<UpdateRecordByIdHandler> _logger;

        private const string UserNotFoundMessage = "User is not found";
        private const string RecordNotFoundMessage = "Record is not found";
        private const string DataChangedMessage = "Data changed, Record: {id}";
        private const string ErrorMessage = "An error occurred while changing the text";

        public UpdateRecordByIdHandler(IRecordRepository recordRepo, IUnitOfWork unitOfWork, IUserRepository userRepo, ILogger<UpdateRecordByIdHandler> logger, IRecordCloudService recordCloudService)
        {
            _recordRepo = recordRepo ?? throw new ArgumentNullException(nameof(recordRepo));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recordCloudService = recordCloudService ?? throw new ArgumentNullException(nameof(recordCloudService));
        }

        public async Task<Result<RecordDto>> Handle(UpdateRecordByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.FetchByIdAsync(request.UserId, cancellationToken);
                if (user is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail(UserNotFoundMessage);
                }

                var record = user.Records.FirstOrDefault(p => p.Id == request.RecordId);

                if (record is null)
                {
                    _logger.LogWarning(RecordNotFoundMessage);
                    return Result.Fail(RecordNotFoundMessage);
                }

                if (request.NewTitle is not null)
                    record.Title = request.NewTitle;

                record.DeadLine = request.NewDeadLine;
                record.IsPrivate = request.NewPrivacyStatus;

                await _recordCloudService.UpdateTextFileInCloudAsync(record.Id.ToString(), request.NewText ?? string.Empty);

                _recordRepo.Update(record);
                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                var response = new RecordDto
                {
                    Id = record.Id,
                    DeadLine = request.NewDeadLine,
                    Text = request.NewText ?? string.Empty,
                    Title = request.NewTitle,
                    IsPrivate = request.NewPrivacyStatus,
                    DateCreated= DateTime.Now,
                };

                _logger.LogInformation(DataChangedMessage, record.Id);

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
