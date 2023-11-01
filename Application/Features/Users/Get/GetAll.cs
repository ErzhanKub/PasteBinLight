using Application.Contracts;
using Domain.Repositories;
using Mapster;

namespace Application.Features.Users.Get;

public record GetAllRequest : IRequest<List<UserDto>> { }

public class GetAllRequestHandler : IRequestHandler<GetAllRequest, List<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<List<UserDto>> Handle(GetAllRequest request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync();

        var response = users.Adapt<List<UserDto>>();

        return response;
    }
}
