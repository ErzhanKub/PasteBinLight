using Application.Contracts;
using Domain.Repositories;

namespace Application.Features.Users.Get;

public record GetAllRequest : IRequest<IReadOnlyList<UserDto>> { }

public class GetAllRequestHandler : IRequestHandler<GetAllRequest, IReadOnlyList<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<IReadOnlyList<UserDto>> Handle(GetAllRequest request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();

        var response = new List<UserDto>();

        foreach (var user in users)
        {
            var result = new UserDto
            {
                Id = user.Id,
                Username = user.Username.Value,
                Email = user.Email.Value,
                Role = user.Role
            };
            response.Add(result);
        }

        return response;
    }
}
