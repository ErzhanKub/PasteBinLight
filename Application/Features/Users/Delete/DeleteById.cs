namespace Application.Features.Users.Delete;

public record DeleteUsersByIdsCommand : IRequest<Result<Guid[]>>
{
    public Guid[]? Id { get; init; }
}

public class DeleteUsersByIdsCommandValidator : AbstractValidator<DeleteUsersByIdsCommand>
{
    public DeleteUsersByIdsCommandValidator()
    {
        RuleFor(i => i.Id)
            .NotEmpty()
            .WithMessage("User Ids cannot be empty");

        RuleForEach(i => i.Id)
            .NotNull()
            .WithMessage("User Id cannot be null");
    }
}

public class DeleteUsersByIdsHandler : IRequestHandler<DeleteUsersByIdsCommand, Result<Guid[]>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUsersByIdsHandler> _logger;

    private const string UserNotFoundMessega = "User(s) not found";
    private const string UserDeletedMessega = "Deleted users: {UserIds}";
    private const string ErrorMessega = "Error occurred while deleting users";
    public DeleteUsersByIdsHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<DeleteUsersByIdsHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Guid[]>> Handle(DeleteUsersByIdsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _userRepository.DeleteRangeAsync(request.Id!);

            if (result is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail<Guid[]>(UserNotFoundMessega);
            }

            await _unitOfWork.SaveCommitAsync();

            _logger.LogInformation(UserDeletedMessega, string.Join(", ", result));
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
