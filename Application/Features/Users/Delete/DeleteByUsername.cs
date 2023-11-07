namespace Application.Features.Users.Delete;

public record DeleteByUsernameCommand : IRequest<Result<string>>
{
    public string? Username { get; init; }
}

public class DeleteByUsernameValidator : AbstractValidator<DeleteByUsernameCommand>
{
    public DeleteByUsernameValidator() 
    {
        RuleFor(u => u.Username)
            .NotNull()
            .WithMessage("Such user does not exist");
        RuleFor(u => u.Username)
            .NotEmpty()
            .WithMessage("Username must not be empty");
    }
}

public class DeleteByUsernameCommadHandler : IRequestHandler<DeleteByUsernameCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUsersByIdsHandler> _logger;

    private const string UserNotFoundMessage = "User(s) not found";
    private const string UserDeletedMessage = "Deleted users: {Username}";
    private const string ErrorMessage = "Error occurred while deleting users";

    public DeleteByUsernameCommadHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeleteUsersByIdsHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<Result<string>> Handle(DeleteByUsernameCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userRepository.DeleteUserByUsernameAsync(request.Username!);

            if (result is null)
            {
                _logger.LogWarning(UserNotFoundMessage);
                return Result.Fail<string>(UserNotFoundMessage);
            }

            await _unitOfWork.SaveCommitAsync();

            _logger.LogInformation(UserDeletedMessage, string.Join(", ", result));
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessage);
            throw;
        }
    }
}
