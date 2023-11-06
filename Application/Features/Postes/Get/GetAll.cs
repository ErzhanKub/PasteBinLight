namespace Application.Features.Postes.Get;

public record GetAllPosteRequest : IRequest<Result<List<GetAllPosteDto>>> { }
public class GetAllPosteRequestValidator : AbstractValidator<GetAllPosteRequest>
{
    public GetAllPosteRequestValidator() { }
}
public class GetAllPosteHandler : IRequestHandler<GetAllPosteRequest, Result<List<GetAllPosteDto>>>
{
    private readonly IPosteRepository _posteRepository;
    private readonly ILogger<GetAllPosteHandler> _logger;

    private const string PosteReceivedMessega = "Received all public poste";
    private const string ErrorMessega = "An error occurred while receiving mail";

    public GetAllPosteHandler(IPosteRepository posteRepository, ILogger<GetAllPosteHandler> logger)
    {
        _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<List<GetAllPosteDto>>> Handle(GetAllPosteRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var allPostes = await _posteRepository.GetAllAsync();
            var publicPostes = allPostes.Where(p => p.IsPrivate != true).ToList();

            var response = publicPostes.Select(poste => new GetAllPosteDto
            {
                Id = poste.Id,
                DateCreated = poste.DateCreated,
                DisLikes = poste.DisLikes,
                Likes = poste.Likes,
                Title = poste.Title,
            }).ToList();

            _logger.LogInformation(PosteReceivedMessega);

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
