using Application.Shared;
using Domain.Repositories;

namespace Application.Features.Users.Delete
{
    public record DeleteUsersByIdsCommand : IRequest<Result<Guid[]>>
    {
        public Guid[]? Id { get; init; }
    }

    public class DeleteUsersByIdsCommandValidator : AbstractValidator<DeleteUsersByIdsCommand>
    {
        public DeleteUsersByIdsCommandValidator()
        {
            RuleFor(i => i.Id).NotEmpty();
        }
    }

    public class DeleteUsersByIdsHandler : IRequestHandler<DeleteUsersByIdsCommand, Result<Guid[]>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUsersByIdsHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Guid[]>> Handle(DeleteUsersByIdsCommand request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.DeleteRangeAsync(request.Id!);

            if (result is null)
                return Result.Fail<Guid[]>("User(s) not found");

            await _unitOfWork.SaveCommitAsync();

            return Result.Ok(result);
        }
    }
}
