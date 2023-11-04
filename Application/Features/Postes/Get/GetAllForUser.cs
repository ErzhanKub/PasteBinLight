using Application.Contracts;
using Domain.Repositories;

namespace Application.Features.Postes.Get
{
    public record GetAllPostsForUserRequest : IRequest<Result<List<GetAllPosteDto>>> 
    {
        public Guid UserId { get; init; }
    }

    public class GetAllPosteForUserRequestValidator : AbstractValidator<GetAllPostsForUserRequest>
    {
        public GetAllPosteForUserRequestValidator() { }
    }

    public class GetAllPostsForUserHandler : IRequestHandler<GetAllPostsForUserRequest, Result<List<GetAllPosteDto>>>
    {
        private readonly IPosteRepository _posteRepository;
        private readonly IUserRepository _userRepository;

        public GetAllPostsForUserHandler(IPosteRepository posteRepository, IUserRepository userRepository)
        {
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Result<List<GetAllPosteDto>>> Handle(GetAllPostsForUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                return Result.Fail("User is not found");

            var allPostes = await _posteRepository.GetAllAsync();

            var response = allPostes.Where(p => p.UserId == user.Id).Select(poste => new GetAllPosteDto
            {
                Id = user.Id,
                DateCreated = poste.DateCreated,
                DisLikes = poste.DisLikes,
                Likes = poste.Likes,
                Title = poste.Title,
            }).ToList();

            return Result.Ok(response);
        }
    }
}
