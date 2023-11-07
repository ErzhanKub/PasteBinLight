using Domain.IServices;

namespace Application.Features.Postes.Update;

public class UpdatePasteByIdCommand : IRequest<Result<PasteDto>>
{
    public Guid UserId { get; set; }
    public Guid PosteId { get; init; }
    public string? Title { get; init; }
    public string? Text { get; init; }
    public bool IsPrivate { get; init; }
    public DateTime DeadLine { get; init; }
}

public class UpdatePasteByIdvalidator : AbstractValidator<UpdatePasteByIdCommand>
{
    public UpdatePasteByIdvalidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(p => p.PosteId).NotEmpty();
        RuleFor(p => p.Title).Length(1, 200);
        RuleFor(p => p.Text).Length(1, 4000);
        RuleFor(p => p.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
    }
}

public class UpdatePasteByIdHandler : IRequestHandler<UpdatePasteByIdCommand, Result<PasteDto>>
{
    private readonly IPasteRepository _pasteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IPasteCloudService _pasteCloudService;
    private readonly ILogger<UpdatePasteByIdHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string PasteNotFoundMessega = "Poste is not found";
    private const string DataChangedMessage = "Data changed, poste: {id}";
    private const string ErrorMessega = "An error occurred while changing the text";

    public UpdatePasteByIdHandler(IPasteRepository repository, IUnitOfWork unitOfWork, IUserRepository userRepository, ILogger<UpdatePasteByIdHandler> logger, IPasteCloudService pasteCloudService)
    {
        _pasteRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pasteCloudService = pasteCloudService ?? throw new ArgumentNullException(nameof(pasteCloudService));
    }

    public async Task<Result<PasteDto>> Handle(UpdatePasteByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var paste = user.Pastes.FirstOrDefault(p => p.Id == request.PosteId);

            if (paste is null)
            {
                _logger.LogWarning(PasteNotFoundMessega);
                return Result.Fail(PasteNotFoundMessega);
            }

            if (request.Title is not null)
                paste.Title = request.Title;

            paste.DeadLine = request.DeadLine;
            paste.IsPrivate = request.IsPrivate;

            await _pasteCloudService.EditTextFromCloudeAsync(paste.Id.ToString(), request.Text ?? string.Empty);

            _pasteRepository.Update(paste);
            await _unitOfWork.SaveCommitAsync();

            var response = new PasteDto
            {
                Id = paste.Id,
                DeadLine = request.DeadLine,
                Text = request.Text ?? string.Empty,
                Title = request.Title,
                IsPrivate = request.IsPrivate,
            };

            _logger.LogInformation(DataChangedMessage, paste.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
