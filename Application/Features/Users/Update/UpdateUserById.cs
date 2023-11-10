// Importing the required libraries
using Domain.Enums;

// Namespace for the user update feature
namespace Application.Features.Users.Update
{
    // Class to handle the command for updating a user by ID
    public record UpdateUserByIdCommand : IRequest<Result<UpdateUserByIdDto>>
    {
        public Guid UserId { get; init; }
        public string? NewUsername { get; init; }
        public string? NewPassword { get; init; }
        public string? NewEmail { get; init; }
        public int NewUserRole { get; init; }
    }

    // Validator class for the UpdateUserByIdCommand
    public class UpdateUserByIdValidator : AbstractValidator<UpdateUserByIdCommand>
    {
        public UpdateUserByIdValidator()
        {
            RuleFor(c => c.NewUsername)
               .Length(1, 100)
               .Matches("^[a-zA-Z0-9]*$")
               .WithMessage("Username can only contain alphanumeric characters");

            RuleFor(c => c.NewPassword)
               .MinimumLength(8)
               .WithMessage("Password must be at least 8 characters long");

            RuleFor(e => e.NewEmail)
                .EmailAddress();

            RuleFor(r => r.NewUserRole).ExclusiveBetween(0, 3);
        }
    }

    // Handler class for the UpdateUserByIdCommand
    public class UpdateUserByIdCommandHandler : IRequestHandler<UpdateUserByIdCommand, Result<UpdateUserByIdDto>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateUserByIdCommandHandler> _logger;

        private const string UserNotFoundMessage = "User is not found";
        private const string DataChangedMessage = "User data has been changed";
        private const string ErrorMessage = "An error occurred while changing user data";

        public UpdateUserByIdCommandHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, ILogger<UpdateUserByIdCommandHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<UpdateUserByIdDto>> Handle(UpdateUserByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.FetchByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail(UserNotFoundMessage);
                }

                if (request.NewUsername is not null)
                    user.UpdateUserUsername(request.NewUsername);

                if (request.NewPassword is not null)
                    user.UpdateUserPassword(request.NewPassword);

                if (request.NewEmail is not null)
                    user.UpdateUserEmail(request.NewEmail, true);

                user.UpdateUserRole((Role)request.NewUserRole);

                _userRepo.Update(user);

                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                _logger.LogInformation(DataChangedMessage);

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
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }
    }
}
