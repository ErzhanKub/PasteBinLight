namespace Application.Features.Postes.Delete;

public record DeletePosteByIdsCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; set; }
    public Guid PosteId { get; init; }
}

public class DeletePosteByIdsCommandValidator : AbstractValidator<DeletePosteByIdsCommand>
{
    public DeletePosteByIdsCommandValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty()
            .NotNull();
        RuleFor(p => p.PosteId)
            .NotEmpty()
            .NotNull();
    }
}

public class DeletePosteByIdsHandler : IRequestHandler<DeletePosteByIdsCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPosteRepository _posteRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePosteByIdsHandler> _logger;

    private const string UserNotFoundMessega = "User not found";
    private const string PosteNotFoundMessega = "Post not found";
    private const string RemovedTextMessega = "Removed text from the cloud database: {id}";
    private const string ChangesSavedMessega = "Changes are saved to the local database";
    private const string ErrorMessega = "An error occurred while deleting mail";

    public DeletePosteByIdsHandler(IUserRepository userRepository, IPosteRepository posteRepository, IUnitOfWork unitOfWork, ILogger<DeletePosteByIdsHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Guid>> Handle(DeletePosteByIdsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var deletedPoste = user.Postes!.FirstOrDefault(p => p.Id == request.PosteId);
            if (deletedPoste == null)
            {
                _logger.LogWarning(PosteNotFoundMessega);
                return Result.Fail<Guid>(PosteNotFoundMessega);
            }

            await _posteRepository.DeleteTextFromCloudAsync(deletedPoste.Id.ToString());
            _logger.LogInformation(RemovedTextMessega, deletedPoste.Id);

            var response = await _posteRepository.DeleteRangeAsync(deletedPoste.Id);
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
