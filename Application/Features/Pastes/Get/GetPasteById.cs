using Domain.IServices;

namespace Application.Features.Postes.Get;

public record GetPasteByIdRequest : IRequest<Result<PasteDto>>
{
    public Guid PasteId { get; init; }
    public Guid UserId { get; init; }
}

public class GetPasteByIdValidator : AbstractValidator<GetPasteByIdRequest>
{
    public GetPasteByIdValidator()
    {
        RuleFor(p => p.PasteId).NotEmpty();
        RuleFor(p => p.UserId).NotEmpty();
    }
}

public sealed class GetPasteByIdHandler : IRequestHandler<GetPasteByIdRequest, Result<PasteDto>>
{
    private readonly IPasteRepository _pasteRepository;
    private readonly IPasteCloudService _pasteCloudService;
    private readonly ILogger<GetPasteByIdHandler> _logger;

    private const string PasteNotFoundMessega = "Paste not found";
    private const string AccessDeniedMessege = "Access denied";
    private const string PasteReceived = "Paste received: {id}";
    private const string ErrorMessage = "An error occurred while receiving the paste";
    public GetPasteByIdHandler(IPasteRepository pasteRepository, IUserRepository userRepository, ILogger<GetPasteByIdHandler> logger, IPasteCloudService pasteCloudService)
    {
        _pasteRepository = pasteRepository ?? throw new ArgumentNullException(nameof(pasteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pasteCloudService = pasteCloudService ?? throw new ArgumentNullException(nameof(pasteCloudService));
    }

    public async Task<Result<PasteDto>> Handle(GetPasteByIdRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var paste = await _pasteRepository.GetByIdAsync(request.PasteId).ConfigureAwait(false);
            if (paste is null)
            {
                _logger.LogWarning(PasteNotFoundMessega);
                return Result.Fail(PasteNotFoundMessega);
            }


            if (paste.IsPrivate && paste.UserId != request.UserId)
            {
                _logger.LogWarning(AccessDeniedMessege);
                return Result.Fail<PasteDto>(AccessDeniedMessege);
            }

            var text = await _pasteCloudService.GetTextFromCloudAsync(paste.Url);

            var response = new PasteDto
            {
                Id = paste.Id,
                Title = paste.Title,
                Text = text,
                DateCreated = paste.DateCreated,
                DeadLine = paste.DeadLine,
                IsPrivate = paste.IsPrivate,
                Likes = paste.Likes,
                DisLikes = paste.DisLikes
            };

            _logger.LogInformation(PasteReceived, paste.Id);
            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}
