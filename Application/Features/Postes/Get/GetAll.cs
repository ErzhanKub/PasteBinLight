namespace Application.Features.Postes.Get
{
    public record GetAllPosteRequest : IRequest<Result<List<GetAllPosteDto>>> { }

    public class GetAllPosteRequestValidator : AbstractValidator<GetAllPosteRequest>
    {
        public GetAllPosteRequestValidator() { }
    }

    public class GetAllPosteHandler : IRequestHandler<GetAllPosteRequest, Result<List<GetAllPosteDto>>>
    {
        private readonly IPosteRepository _posteRepository;

        public GetAllPosteHandler(IPosteRepository posteRepository)
        {
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
        }

        public async Task<Result<List<GetAllPosteDto>>> Handle(GetAllPosteRequest request, CancellationToken cancellationToken)
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

            return Result.Ok(response);
        }
    }
}
