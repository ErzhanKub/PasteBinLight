using Domain.IServices;

namespace Application.Features.Records.Update
{
    // Class to handle the command for updating a record by ID
    public record UpdateRecordByIdCommand : IRequest<Result<RecordDto>>
    {
        public Guid UserId { get; init; }
        public UpdateRecordDto? Data { get; init; }
    }

    public record UpdateRecordDto
    {
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

            When(u => u.Data != null, () =>
            {
                RuleFor(t => t.Data!.NewText).NotEmpty().Length(1, 4000);
                RuleFor(t => t.Data!.NewTitle).Length(1, 200);
                RuleFor(d => d.Data!.NewDeadLine).GreaterThanOrEqualTo(DateTime.Now);
            });
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

                var record = user.Records.FirstOrDefault(p => p.Id == request.Data!.RecordId);

                if (record is null)
                {
                    _logger.LogWarning(RecordNotFoundMessage);
                    return Result.Fail(RecordNotFoundMessage);
                }

                if (request.Data!.NewTitle is not null)
                    record.Title = request.Data!.NewTitle;

                record.DeadLine = request.Data!.NewDeadLine;
                record.IsPrivate = request.Data!.NewPrivacyStatus;

                await _recordCloudService.UpdateTextFileInCloudAsync(record.Id.ToString(), request.Data!.NewText ?? string.Empty);

                _recordRepo.Update(record);
                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                var response = new RecordDto
                {
                    Id = record.Id,
                    DeadLine = request.Data!.NewDeadLine,
                    Text = request.Data!.NewText ?? string.Empty,
                    Title = request.Data!.NewTitle,
                    IsPrivate = request.Data!.NewPrivacyStatus,
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
