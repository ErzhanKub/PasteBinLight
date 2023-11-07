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

public class UpdatePosteByIdvalidator : AbstractValidator<UpdatePasteByIdCommand>
{
    public UpdatePosteByIdvalidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(p => p.PosteId).NotEmpty();
        RuleFor(p => p.Title).Length(1, 200);
        RuleFor(p => p.Text).Length(1, 4000);
        RuleFor(p => p.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
    }
}

public class UpdatePosteByIdHandler : IRequestHandler<UpdatePasteByIdCommand, Result<PasteDto>>
{
    private readonly IPasteRepository _posteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdatePosteByIdHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string PosteNotFoundMessega = "Poste is not found";
    private const string DataChangedMessage = "Data changed, poste: {id}";
    private const string ErrorMessega = "An error occurred while changing the text";

    public UpdatePosteByIdHandler(IPasteRepository repository, IUnitOfWork unitOfWork, IUserRepository userRepository, ILogger<UpdatePosteByIdHandler> logger)
    {
        _posteRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            var poste = user.Postes.FirstOrDefault(p => p.Id == request.PosteId);

            if (poste is null)
            {
                _logger.LogWarning(PosteNotFoundMessega);
                return Result.Fail(PosteNotFoundMessega);
            }

            if (request.Title is not null)
                poste.Title = request.Title;

            poste.DeadLine = request.DeadLine;
            poste.IsPrivate = request.IsPrivate;

            await _posteRepository.EditTextFromCloudeAsync(poste.Id.ToString(), request.Text ?? string.Empty);

            _posteRepository.Update(poste);
            await _unitOfWork.SaveCommitAsync();

            var response = new PasteDto
            {
                Id = poste.Id,
                DeadLine = request.DeadLine,
                Text = request.Text ?? string.Empty,
                Title = request.Title,
                IsPrivate = request.IsPrivate,
            };

            _logger.LogInformation(DataChangedMessage, poste.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
