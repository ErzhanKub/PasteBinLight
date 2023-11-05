namespace Application.Features.Postes.Create;

public record CreatePosteCommand : IRequest<Result<string>>
{
    public Guid UserId { get; set; }
    public required string Text { get; init; }
    public string? Title { get; init; }
    public DateTime DeadLine { get; init; }
    public bool IsPrivate { get; init; }
}

public class CreatePosteCommandValidator : AbstractValidator<CreatePosteCommand>
{
    public CreatePosteCommandValidator()
    {
        RuleFor(u => u.UserId).NotEmpty();
        RuleFor(t => t.Text).NotEmpty().Length(1, 4000);
        RuleFor(t => t.Title).Length(1, 200);
        RuleFor(d => d.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
    }
}

public sealed class CreatePosteHandler : IRequestHandler<CreatePosteCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPosteRepository _posteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePosteHandler> _logger;
    
    private const string UserNotFoundMessage = "User not found";
    private const string LoadingTextMessage = "Loading text into the cloud database: {Id}";
    private const string AddedPosteMessage = "Added poste to user";
    private const string PosteSavedMessage = "Poste saved to local database, posteId: {posteId}; userId: {userId}";
    private const string ErrorMessage = "An error occurred during poste creation";

    public CreatePosteHandler(IUserRepository userRepository, IPosteRepository posteRepository, IUnitOfWork unitOfWork, ILogger<CreatePosteHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> Handle(CreatePosteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessage);
                return Result.Fail(UserNotFoundMessage);
            }

            var posteId = Guid.NewGuid();

            var urlCloud = await _posteRepository.UploadTextToCloudAsync(posteId.ToString(), request.Text);

            _logger.LogInformation(LoadingTextMessage, posteId);

            var poste = new Poste
            {
                Id = posteId,
                DateCreated = DateTime.Now,
                DeadLine = request.DeadLine,
                IsPrivate = request.IsPrivate,
                Title = request.Title,
                Url = new Uri(urlCloud),
                User = user,
                UserId = user.Id,
            };

            user.AddPoste(poste);
            _logger.LogInformation(AddedPosteMessage);

            var guid = await _posteRepository.CreateAsync(poste);

            _userRepository.Update(user);

            await _unitOfWork.SaveCommitAsync();
            _logger.LogInformation(PosteSavedMessage, poste.Id, user.Id);

            var encodedGuid = _posteRepository.GetEncodedGuid(guid);

            return Result.Ok(encodedGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}
