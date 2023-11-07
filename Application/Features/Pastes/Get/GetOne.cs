using Domain.IServices;

namespace Application.Features.Postes.Get;

public record GetOnePasteByUrlRequest : IRequest<Result<PasteDto>>
{
    public string? EncodedGuid { get; init; }
    public Guid UserId { get; init; }
}

public class GetOnePasteByUrlValidator : AbstractValidator<GetOnePasteByUrlRequest>
{
    public GetOnePasteByUrlValidator()
    {
        RuleFor(u => u.EncodedGuid).NotEmpty();
    }
}

public class GetOnePasteByUrlHandler : IRequestHandler<GetOnePasteByUrlRequest, Result<PasteDto>>
{
    private readonly IPasteRepository _pasteRepository;
    private readonly IPasteCloudService _pasteCloudService;
    private readonly ILogger<GetOnePasteByUrlHandler> _logger;

    private const string PasteNotFoundMessega = "Poste not found";
    private const string AccessDeniedMessega = "Access denied";
    private const string TextReceived = "Text received: {id}";
    private const string ErrorMessega = "An error occurred while receiving the text";

    public GetOnePasteByUrlHandler(IPasteRepository pasteRepository, ILogger<GetOnePasteByUrlHandler> logger, IPasteCloudService pasteCloudService)
    {
        _pasteRepository = pasteRepository ?? throw new ArgumentNullException(nameof(pasteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _pasteCloudService = pasteCloudService ?? throw new ArgumentNullException(nameof(pasteCloudService));
    }

    public async Task<Result<PasteDto>> Handle(GetOnePasteByUrlRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var pasteId = _pasteRepository.GetDecodedGuid(request.EncodedGuid!);

            var paste = await _pasteRepository.GetByIdAsync(pasteId);

            if (paste is null)
            {
                _logger.LogWarning(PasteNotFoundMessega);
                return Result.Fail(PasteNotFoundMessega);
            }

            if (paste.IsPrivate && paste.UserId != request.UserId)
            {
                _logger.LogWarning(AccessDeniedMessega);
                return Result.Fail<PasteDto>(AccessDeniedMessega);
            }


            var text = await _pasteCloudService.GetTextFromCloudAsync(paste.Url);

            var response = new PasteDto
            {
                Id = paste.Id,
                DateCreated = paste.DateCreated,
                DeadLine = paste.DeadLine,
                DisLikes = paste.DisLikes,
                IsPrivate = paste.IsPrivate,
                Likes = paste.Likes,
                Text = text ?? string.Empty,
                Title = paste.Title,
            };

            _logger.LogInformation(TextReceived, paste.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
