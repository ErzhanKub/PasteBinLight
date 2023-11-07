using Domain.Entities;

namespace Application.Features.Postes.Get
{
    public record GetPasteByIdRequest : IRequest<Result<PosteDto>>
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

    public sealed class GetPasteByIdHandler : IRequestHandler<GetPasteByIdRequest, Result<PosteDto>>
    {
        private readonly IPosteRepository _posteRepository;
        private readonly ILogger<GetPasteByIdHandler> _logger;

        private const string PasteNotFoundMessega = "Paste not found";
        private const string AccessDeniedMessege = "Access denied";
        private const string PasteReceived = "Paste received: {id}";
        private const string ErrorMessage = "An error occurred while receiving the paste";
        public GetPasteByIdHandler(IPosteRepository posteRepository, IUserRepository userRepository, ILogger<GetPasteByIdHandler> logger)
        {
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<PosteDto>> Handle(GetPasteByIdRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var paste = await _posteRepository.GetByIdAsync(request.PasteId).ConfigureAwait(false);
                if (paste is null)
                {
                    _logger.LogWarning(PasteNotFoundMessega);
                    return Result.Fail(PasteNotFoundMessega);
                }


                if (paste.IsPrivate && paste.UserId != request.UserId)
                {
                    _logger.LogWarning(AccessDeniedMessege);
                    return Result.Fail<PosteDto>(AccessDeniedMessege);
                }

                var text = await _posteRepository.GetTextFromCloudAsync(paste.Url);

                var response = new PosteDto
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
}
