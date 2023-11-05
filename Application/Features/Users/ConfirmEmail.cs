namespace Application.Features.Users
{
    public record ConfirmEmailCommand : IRequest<Result>
    {
        public Guid UserId { get; init; }
        public string? ConfirmToken { get; init; }
    }

    public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(u => u.UserId).NotEmpty();
            RuleFor(t => t.ConfirmToken).NotEmpty();
        }
    }

    public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmEmailHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user is null)
                return Result.Fail("User not found");

            if (user.ConfirmationToken == request.ConfirmToken)
                user.Email.EmailConfirmed = true;

            _userRepository.Update(user);
            await _unitOfWork.SaveCommitAsync();

            return Result.Ok();
        }
    }
}
