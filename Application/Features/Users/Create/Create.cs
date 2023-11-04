using Application.Shared;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Features.Users.Create
{
    public record CreateUserCommand : IRequest<Result<Guid>>
    {
        public required string Username { get; init; }
        public required string Password { get; init; }
        public required string Email { get; init; }
    }

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(c => c.Username).NotEmpty().Length(0, 200);
            RuleFor(c => c.Password).NotEmpty().Length(0, 200);
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
        }
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = User.Create(new Username(request.Username),
                new Password(request.Password),
                new Email(request.Email),
                Domain.Enums.Role.User);

            await _userRepository.CreateAsync(user);

            await _unitOfWork.SaveCommitAsync();

            return Result.Ok(user.Id);
        }
    }
}
