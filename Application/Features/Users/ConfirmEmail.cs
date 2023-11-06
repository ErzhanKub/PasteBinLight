namespace Application.Features.Users;

public record ConfirmEmailCommand : IRequest<Result>
{
    public Guid UserId { get; init; }
    public string? ConfirmToken { get; init; }
}

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(u => u.UserId)
            .NotEmpty();
        RuleFor(t => t.ConfirmToken)
            .NotEmpty()
            .NotNull();
    }
}

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfirmEmailHandler> _logger;

    private const string UserNotFoundMessega = "User is not found";
    private const string EmailConfirmedMessega = "Email has been confirmed: {email}";
    private const string ErrorMessega = "An error occurred during email confirmation";

    public ConfirmEmailHandler(IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger<ConfirmEmailHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                _logger.LogWarning(UserNotFoundMessega);
                return Result.Fail(UserNotFoundMessega);
            }
                
            if (user.ConfirmationToken != request.ConfirmToken)
            {
                _logger.LogWarning("Email was not confirmed: {email}", user.Email.Value);
                return Result.Fail("Email was not confirmed");
            }

            user.Email.EmailConfirmed = true;
            _userRepository.Update(user);
            await _unitOfWork.SaveCommitAsync();

            _logger.LogInformation(EmailConfirmedMessega, user.Email.Value);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ErrorMessega);
            throw;
        }
    }
}
