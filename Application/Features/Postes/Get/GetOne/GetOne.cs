using Domain.Repositories;

namespace Application.Features.Postes.Get.GetOne
{
    public class GetOnePosteByUrlRequest : IRequest<Result<GetOnePosteDto>>
    {
        public Uri? Url { get; init; }
    }

    public class GetOnePosteByUrlValidator : AbstractValidator<GetOnePosteByUrlRequest>
    {
        public GetOnePosteByUrlValidator()
        {
            RuleFor(u => u.Url).NotEmpty();
        }
    }

    public class GetOnePosteByUrlHandler : IRequestHandler<GetOnePosteByUrlRequest, Result<GetOnePosteDto>>
    {
        private readonly IPosteRepository _posteRepository;

        public GetOnePosteByUrlHandler(IPosteRepository posteRepository)
        {
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        }

        public async Task<Result<GetOnePosteDto>> Handle(GetOnePosteByUrlRequest request, CancellationToken cancellationToken)
        {
            string[] parts = request.Url!.ToString().Split('/');

            string key = parts.Last();

            var posteId = _posteRepository.GetDecodedGuid(key);

            var poste = await _posteRepository.GetByIdAsync(posteId);
            if (poste == null)
                return Result.Fail("Poste not found");

            var text = await _posteRepository.GetTextFromCloudAsync(poste.Url);

            var response = new GetOnePosteDto
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

            return Result.Ok(response);
        }
    }
}
