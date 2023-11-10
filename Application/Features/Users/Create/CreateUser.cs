// Namespace for the user creation feature
namespace Application.Features.Users.Create
{
    // Class to handle the command for creating a user
    public record CreateUserCommand : IRequest<Result<UserDto>>
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
        public required string Email { get; init; }
    }

    // Validator class for the CreateUserCommand
    public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
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

    // Handler class for the CreateUserCommand
    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        private const string UserCreatedMessage = "User created: {id}";
        private const string ErrorMessage = "An error occurred while creating users";

        public CreateUserCommandHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, ILogger<CreateUserCommandHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var token = _userRepo.GenerateEmailConfirmationToken();

                var user = User.CreateUser(new Username(request.Username),
                    new Password(_userRepo.HashPassword(request.Password)),
                    new Email(request.Email, false),
                    Domain.Enums.Role.User,
                    token);

                await _userRepo.CreateAsync(user, cancellationToken);
                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                _logger.LogInformation(UserCreatedMessage, user.Id);

                await _userRepo.SendEmailAsync(user.Email.Value, token);

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
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }
    }
}
