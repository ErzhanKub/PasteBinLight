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
        private readonly IUserRepository _userRepository;

        public GetAllPostsForUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<Result<List<GetAllPosteDto>>> Handle(GetAllPostsForUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                return Result.Fail("User is not found");

            var response = user.Postes.Select(poste => new GetAllPosteDto
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
