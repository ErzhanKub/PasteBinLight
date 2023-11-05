namespace Application.Features.Postes.Get
{
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
        public GetOnePosteByUrlHandler(IPosteRepository posteRepository)
        {
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        }

        public async Task<Result<PosteDto>> Handle(GetOnePosteByUrlRequest request, CancellationToken cancellationToken)
        {
            var posteId = _posteRepository.GetDecodedGuid(request.EncodedGuid!);

            var poste = await _posteRepository.GetByIdAsync(posteId);

            if (poste == null)
                return Result.Fail("Poste not found");

            if (poste.IsPrivate && poste.UserId != request.UserId)
                return Result.Fail<PosteDto>("Access denied");


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

            return Result.Ok(response);
        }
    }
}
