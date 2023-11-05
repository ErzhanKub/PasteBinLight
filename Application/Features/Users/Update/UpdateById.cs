using Domain.Enums;
namespace Application.Features.Users.Update;
public record UpdateUserByIdCommand : IRequest<Result<UpdateUserByIdDto>>
{
    public Guid Id { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string? Email { get; init; }
    public int UserRole { get; init; }
}

public class UpdateUserByIdValidator : AbstractValidator<UpdateUserByIdCommand>
{
    public UpdateUserByIdValidator()
    {
        RuleFor(c => c.Username)
           .Length(1, 100)
           .Matches("^[a-zA-Z0-9]*$")
           .WithMessage("Username can only contain alphanumeric characters");

        RuleFor(c => c.Password)
           .MinimumLength(8)
           .WithMessage("Password must be at least 8 characters long");

        RuleFor(e => e.Email)
            .EmailAddress();

        RuleFor(r => r.UserRole).ExclusiveBetween(0, 3);
    }
}

public class UpdateUserByIdCommandHandler : IRequestHandler<UpdateUserByIdCommand, Result<UpdateUserByIdDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UpdateUserByIdCommandHandler> _logger;

    public UpdateUserByIdCommandHandler(IUserRepository userRepository, IUnitOfWork uow, ILogger<UpdateUserByIdCommandHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<UpdateUserByIdDto>> Handle(UpdateUserByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.Id);

            if (user == null)
            {
                _logger.LogWarning("User is not found");
                return Result.Fail("User is not found");
            }

            if (request.Username is not null)
                user.UpdateUsername(request.Username);

            if (request.Password is not null)
                user.UpdatePassword(request.Password);

            if (request.Email is not null)
                user.UpdateEmail(request.Email, true);

            user.UpdateRole((Role)request.UserRole);

            _userRepository.Update(user);

            await _uow.SaveCommitAsync();

            _logger.LogInformation("User data has been changed");

            var response = new UpdateUserByIdDto
            {
                Email = user.Email.Value,
                Id = user.Id,
                Username = user.Username.Value,
                Role = user.Role,
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while changing user data");
            throw;
        }
    }
}
