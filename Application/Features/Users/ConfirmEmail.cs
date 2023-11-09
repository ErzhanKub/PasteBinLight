// Namespace for the user email confirmation feature
namespace Application.Features.Users
{
    // Class to handle the command for confirming a user's email
    public record ConfirmUserEmailCommand : IRequest<Result>
    {
        public Guid UserId { get; init; }
        public string? ConfirmationToken { get; init; }
    }

    // Validator class for the ConfirmUserEmailCommand
    public class ConfirmUserEmailValidator : AbstractValidator<ConfirmUserEmailCommand>
    {
        public ConfirmUserEmailValidator()
        {
            RuleFor(u => u.UserId)
                .NotEmpty();
            RuleFor(t => t.ConfirmationToken)
                .NotEmpty()
                .NotNull();
        }
    }

    // Handler class for the ConfirmUserEmailCommand
    public class ConfirmUserEmailHandler : IRequestHandler<ConfirmUserEmailCommand, Result>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConfirmUserEmailHandler> _logger;

        private const string UserNotFoundMessage = "User is not found";
        private const string EmailConfirmedMessage = "Email has been confirmed: {email}";
        private const string ErrorMessage = "An error occurred during email confirmation";

        public ConfirmUserEmailHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, ILogger<ConfirmUserEmailHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> Handle(ConfirmUserEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepo.FetchByIdAsync(request.UserId, cancellationToken);

                if (user is null)
                {
                    _logger.LogWarning(UserNotFoundMessage);
                    return Result.Fail(UserNotFoundMessage);
                }

                if (user.ConfirmationToken != request.ConfirmationToken)
                {
                    _logger.LogWarning("Email was not confirmed: {email}", user.Email.Value);
                    return Result.Fail("Email was not confirmed");
                }

                user.Email.EmailConfirmed = true;
                _userRepo.Update(user);
                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                _logger.LogInformation(EmailConfirmedMessage, user.Email.Value);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMessage);
                throw;
            }
        }
    }
}
