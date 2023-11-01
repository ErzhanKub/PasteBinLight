using Application.Contracts;
using Domain.Repositories;

namespace Application.Features.Users.Get;

public record GetOneUserRequest : IRequest<Result<UserDto>>
{
    public Guid Id { get; init; }
}

public class GetOneUserValidator : AbstractValidator<GetOneUserRequest>
{
    public GetOneUserValidator()
    {
        RuleFor(i => i.Id).NotEmpty();
    }
}

public class GetOneUserRequestHandler : IRequestHandler<GetOneUserRequest, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetOneUserRequestHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<Result<UserDto>> Handle(GetOneUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
            return Result.Fail<UserDto>("User not found");

        var response = new UserDto
        { 
            Id = request.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
        };
        
        return Result.Ok(response);
    }
}
