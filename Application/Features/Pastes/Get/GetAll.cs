namespace Application.Features.Postes.Get;

public record GetAllPasteRequest : IRequest<Result<List<GetAllPasteDto>>> { }
public class GetAllPasteRequestValidator : AbstractValidator<GetAllPasteRequest>
{
    public GetAllPasteRequestValidator() { }
}
public class GetAllPasteHandler : IRequestHandler<GetAllPasteRequest, Result<List<GetAllPasteDto>>>
{
    private readonly IPasteRepository _pasteRepository;
    private readonly ILogger<GetAllPasteHandler> _logger;

    private const string PasteReceivedMessega = "Received all public poste";
    private const string ErrorMessega = "An error occurred while receiving mail";

    public GetAllPasteHandler(IPasteRepository pasteRepository, ILogger<GetAllPasteHandler> logger)
    {
        _pasteRepository = pasteRepository ?? throw new ArgumentNullException(nameof(pasteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllPasteDto>>> Handle(GetAllPasteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var allPastes = await _pasteRepository.GetAllAsync();
            var publicPostes = allPastes.Where(p => p.IsPrivate != true).ToList();

            var response = publicPostes.Select(paste => new GetAllPasteDto
            {
                Id = paste.Id,
                DateCreated = paste.DateCreated,
                DisLikes = paste.DisLikes,
                Likes = paste.Likes,
                Title = paste.Title,
            }).ToList();

            _logger.LogInformation(PasteReceivedMessega);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
