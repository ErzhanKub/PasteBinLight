using Domain.IServices;

namespace Application.Features.Postes.Delete;

public record DeletePasteByIdsCommand : IRequest<Result<Guid>>
{
    public Guid UserId { get; set; }
    public Guid PasteId { get; init; }
}

public class DeletePasteByIdsCommandValidator : AbstractValidator<DeletePasteByIdsCommand>
{
    public DeletePasteByIdsCommandValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty()
            .NotNull();
        RuleFor(p => p.PasteId)
            .NotEmpty()
            .NotNull();
    }
}

public class DeletePasteByIdsHandler : IRequestHandler<DeletePasteByIdsCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasteRepository _pasteRepository;
    private readonly IPasteCloudService _pasteCloudService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePasteByIdsHandler> _logger;

    private const string UserNotFoundMessega = "User not found";
    private const string PasteNotFoundMessega = "Post not found";
    private const string RemovedTextMessega = "Removed text from the cloud database: {id}";
    private const string ChangesSavedMessega = "Changes are saved to the local database";
    private const string ErrorMessega = "An error occurred while deleting mail";

    public DeletePasteByIdsHandler(IUserRepository userRepository, IPasteRepository pasteRepository, IUnitOfWork unitOfWork, ILogger<DeletePasteByIdsHandler> logger, IPasteCloudService pasteCloudService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _pasteRepository = pasteRepository ?? throw new ArgumentNullException(nameof(pasteRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pasteCloudService = pasteCloudService ?? throw new ArgumentNullException(nameof(pasteCloudService));
    }

    public async Task<Result<Guid>> Handle(DeletePasteByIdsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }

            var deletedPaste = user.Pastes!.FirstOrDefault(p => p.Id == request.PasteId);
            if (deletedPaste == null)
            {
                _logger.LogWarning(PasteNotFoundMessega);
                return Result.Fail<Guid>(PasteNotFoundMessega);
            }

            await _pasteCloudService.DeleteTextFromCloudAsync(deletedPaste.Id.ToString());
            _logger.LogInformation(RemovedTextMessega, deletedPaste.Id);

            var response = await _pasteRepository.DeleteRangeAsync(deletedPaste.Id);
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
