// Namespace for the user deletion feature
namespace Application.Features.Users.Delete
{
    // Class to handle the command for deleting users by IDs
    public record DeleteUsersByIdsCommand : IRequest<Result<Guid[]>>
    {
        public Guid[]? UserIds { get; init; }
    }

    // Validator class for the DeleteUsersByIdsCommand
    public class DeleteUsersByIdsCommandValidator : AbstractValidator<DeleteUsersByIdsCommand>
    {
        public DeleteUsersByIdsCommandValidator()
        {
            RuleFor(i => i.UserIds)
                .NotEmpty()
                .WithMessage("User Ids cannot be empty");

            RuleForEach(i => i.UserIds)
                .NotNull()
                .WithMessage("User Id cannot be null");
        }
    }

    // Handler class for the DeleteUsersByIdsCommand
    public class DeleteUsersByIdsHandler : IRequestHandler<DeleteUsersByIdsCommand, Result<Guid[]>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteUsersByIdsHandler> _logger;

        private const string UsersNotFoundMessage = "User(s) not found";
        private const string UsersDeletedMessage = "Deleted users: {UserIds}";
        private const string ErrorMessage = "Error occurred while deleting users";

        public DeleteUsersByIdsHandler(IUserRepository userRepo, IUnitOfWork unitOfWork, ILogger<DeleteUsersByIdsHandler> logger)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<Guid[]>> Handle(DeleteUsersByIdsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _userRepo.RemoveByIdAsync(request.UserIds!);

                if (result is null)
                {
                    _logger.LogWarning(UsersNotFoundMessage);
                    return Result.Fail<Guid[]>(UsersNotFoundMessage);
                }

                await _unitOfWork.SaveAndCommitAsync(cancellationToken);

                _logger.LogInformation(UsersDeletedMessage, string.Join(", ", result));
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
