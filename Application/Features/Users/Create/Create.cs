namespace Application.Features.Users.Create;
public record CreateUserCommand : IRequest<Result<UserDto>>
{
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Email { get; init; }
}

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(c => c.Username)
            .NotEmpty()
            .Length(1, 100)
            .Matches("^[a-zA-Z0-9]*$")
            .WithMessage("Username can only contain alphanumeric characters");

        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .Length(1, 16);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    private const string UserCreatedMessega = "User created: {id}";
    private const string ErrorMessega = "An error occurred while creating users";
    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var token = _userRepository.GenerateEmailConfirmationToken();

            var user = User.Create(new Username(request.Username),
                new Password(_userRepository.HashPassword(request.Password)),
                new Email(request.Email, false),
                Domain.Enums.Role.User,
                token);

            await _userRepository.CreateAsync(user);
            await _unitOfWork.SaveCommitAsync();

            _logger.LogInformation(UserCreatedMessega, user.Id);

            await _userRepository.SendEmail(user.Email.Value, token);

            var response = new UserDto
            {
                Id = user.Id,
                Username = user.Username.Value,
                Email = user.Email.Value,
                Role = user.Role,
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
