using Application.Shared;
using Domain.Repositories;

namespace Application.Features.Users.Update;

public record UpdateUserByIdCommand : IRequest<Result<UpdateUserByIdDto>>
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? Email { get; init; }
}

public class UpdateUserByIdValidator : AbstractValidator<UpdateUserByIdDto>
{
    public UpdateUserByIdValidator()
    {
        RuleFor(u => u.Username).NotEmpty().Length(2, 200);
        RuleFor(p => p.Password).NotEmpty().Length(5, 200);
        RuleFor(e => e.Email).EmailAddress().NotEmpty();
    }
}

public class UpdateUserByIdCommandHandler : IRequestHandler<UpdateUserByIdCommand, Result<UpdateUserByIdDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;

    public UpdateUserByIdCommandHandler(IUserRepository userRepository, IUnitOfWork uow)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task<Result<UpdateUserByIdDto>> Handle(UpdateUserByIdCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user == null)
            return Result.Fail("User is not found");
        if (request.Username != null)
            user.Username = request.Username;
        if (request.Password != null)
            user.Password = request.Password;
        if (request.Email != null)
            user.Email = request.Email;

        _userRepository.Update(user);

        await _uow.SaveCommitAsync();

        var response = new UpdateUserByIdDto
        {
            Email = user.Email,
            Password = user.Password,
            Id = user.Id,
            Username = user.Username,
            Role = user.Role,
        };

        return Result.Ok(response);
    }
}
