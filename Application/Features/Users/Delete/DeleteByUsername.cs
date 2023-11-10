// Namespace for the user deletion feature
namespace Application.Features.Users.Delete
{
    // Class to handle the command for deleting a user by username
    public record DeleteByUsernameCommand : IRequest<Result<string>>
    {
        public string? TargetUsername { get; init; }
    }

    // Validator class for the DeleteByUsernameCommand
    public class DeleteByUsernameValidator : AbstractValidator<DeleteByUsernameCommand>
    {
        public DeleteByUsernameValidator()
        {
            RuleFor(u => u.TargetUsername)
                .NotNull()
                .WithMessage("Such user does not exist");
            RuleFor(u => u.TargetUsername)
                .NotEmpty()
                .WithMessage("Username must not be empty");
        }
    }

    // Handler class for the DeleteByUsernameCommand
    public class DeleteByUsernameCommandHandler : IRequestHandler<DeleteByUsernameCommand, Result<string>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteByUsernameCommandHandler> _logger;

        private const string UserNotFoundMessage = "User(s) not found";
        private const string UserDeletedMessage = "Deleted user: {Username}";
        private const string ErrorMessage = "Error occurred while deleting user";

        public DeleteByUsernameCommandHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, ILogger<DeleteByUsernameCommandHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<string>> Handle(DeleteByUsernameCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepo.RemoveUserByUsernameAsync(request.TargetUsername!);

                if (result is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail<string>(UserNotFoundMessage);
                }

                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                _logger.LogInformation(UserDeletedMessage, result);
                return Result.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }
    }
}
