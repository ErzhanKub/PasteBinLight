namespace Application.Features.Postes.Get;

public record GetOnePosteByUrlRequest : IRequest<Result<PosteDto>>
{
    public string? EncodedGuid { get; init; }
    public Guid UserId { get; init; }
}

public class GetOnePosteByUrlValidator : AbstractValidator<GetOnePosteByUrlRequest>
{
    public GetOnePosteByUrlValidator()
    {
        RuleFor(u => u.EncodedGuid).NotEmpty();
    }
}

public class GetOnePosteByUrlHandler : IRequestHandler<GetOnePosteByUrlRequest, Result<PosteDto>>
{
    private readonly IPosteRepository _posteRepository;
    private readonly ILogger<GetOnePosteByUrlHandler> _logger;

    private const string PosteNotFoundMessega = "Poste not found";
    private const string AccessDeniedMessega = "Access denied";
    private const string TextReceived = "Text received: {id}";
    private const string ErrorMessega = "An error occurred while receiving the text";

    public GetOnePosteByUrlHandler(IPosteRepository posteRepository, ILogger<GetOnePosteByUrlHandler> logger)
    {
        _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<PosteDto>> Handle(GetOnePosteByUrlRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var posteId = _posteRepository.GetDecodedGuid(request.EncodedGuid!);

            var poste = await _posteRepository.GetByIdAsync(posteId);

            if (poste is null)
            {
                _logger.LogWarning(PosteNotFoundMessega);
                return Result.Fail(PosteNotFoundMessega);
            }

            if (poste.IsPrivate && poste.UserId != request.UserId)
            {
                _logger.LogWarning(AccessDeniedMessega);
                return Result.Fail<PosteDto>(AccessDeniedMessega);
            }


            var text = await _posteRepository.GetTextFromCloudAsync(poste.Url);

            var response = new PosteDto
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
