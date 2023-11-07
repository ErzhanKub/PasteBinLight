using Domain.IServices;

namespace Application.Features.Postes.Create;

public record CreatePasteCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public required string Text { get; init; }
    public string? Title { get; init; }
    public DateTime DeadLine { get; init; }
    public bool IsPrivate { get; init; }
}

public class CreatePasteCommandValidator : AbstractValidator<CreatePasteCommand>
{
    public CreatePasteCommandValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(t => t.Text).NotEmpty().Length(1, 4000);
        RuleFor(t => t.Title).Length(1, 200);
        RuleFor(d => d.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
    }
}

public sealed class CreatePasteHandler : IRequestHandler<CreatePasteCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasteRepository _pasteRepository;
    private readonly IPasteCloudService _pasteCloudService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePasteHandler> _logger;
    
    private const string UserNotFoundMessage = "User not found";
    private const string LoadingTextMessage = "Loading text into the cloud database: {Id}";
    private const string AddedPasteMessage = "Added poste to user";
    private const string PasteSavedMessage = "Poste saved to local database, posteId: {posteId}; userId: {userId}";
    private const string ErrorMessage = "An error occurred during poste creation";

    public CreatePasteHandler(IUserRepository userRepository, IPasteRepository pasteRepository, IUnitOfWork unitOfWork, ILogger<CreatePasteHandler> logger, IPasteCloudService pasteCloudService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _pasteRepository = pasteRepository ?? throw new ArgumentNullException(nameof(pasteRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pasteCloudService = pasteCloudService ?? throw new ArgumentNullException(nameof(pasteCloudService));
    }

    public async Task<Result<string>> Handle(CreatePasteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessage);
                return Result.Fail(UserNotFoundMessage);
            }

            var pasteId = Guid.NewGuid();

            var urlCloud = await _pasteCloudService.UploadTextToCloudAsync(pasteId.ToString(), request.Text);

            _logger.LogInformation(LoadingTextMessage, pasteId);

            var paste = new Paste
            {
                Id = pasteId,
                DateCreated = DateTime.Now,
                DeadLine = request.DeadLine,
                IsPrivate = request.IsPrivate,
                Title = request.Title,
                Url = new Uri(urlCloud),
                User = user,
                UserId = user.Id,
            };

            user.AddPaste(paste);
            _logger.LogInformation(AddedPasteMessage);

            var guid = await _pasteRepository.CreateAsync(paste);

            _userRepository.Update(user);

            await _unitOfWork.SaveCommitAsync();
            _logger.LogInformation(PasteSavedMessage, paste.Id, user.Id);

            var encodedGuid = _pasteRepository.GetEncodedGuid(guid);

            return Result.Ok(encodedGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}
