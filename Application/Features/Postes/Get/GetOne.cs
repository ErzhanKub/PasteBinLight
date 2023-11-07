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
    private readonly ILogger<GetOnePasteByUrlHandler> _logger;

    private const string PosteNotFoundMessega = "Poste not found";
    private const string AccessDeniedMessega = "Access denied";
    private const string TextReceived = "Text received: {id}";
    private const string ErrorMessega = "An error occurred while receiving the text";

    public GetOnePasteByUrlHandler(IPasteRepository posteRepository, ILogger<GetOnePasteByUrlHandler> logger)
    {
        _pasteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PasteDto>> Handle(GetOnePasteByUrlRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var posteId = _pasteRepository.GetDecodedGuid(request.EncodedGuid!);

            var poste = await _pasteRepository.GetByIdAsync(posteId);

            if (poste is null)
            {
                _logger.LogWarning(PosteNotFoundMessega);
                return Result.Fail(PosteNotFoundMessega);
            }

            if (poste.IsPrivate && poste.UserId != request.UserId)
            {
                _logger.LogWarning(AccessDeniedMessega);
                return Result.Fail<PasteDto>(AccessDeniedMessega);
            }


            var text = await _pasteRepository.GetTextFromCloudAsync(poste.Url);

            var response = new PasteDto
            {
                Id = poste.Id,
                DateCreated = poste.DateCreated,
                DeadLine = poste.DeadLine,
                DisLikes = poste.DisLikes,
                IsPrivate = poste.IsPrivate,
                Likes = poste.Likes,
                Text = text ?? string.Empty,
                Title = poste.Title,
            };

            _logger.LogInformation(TextReceived, poste.Id);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
