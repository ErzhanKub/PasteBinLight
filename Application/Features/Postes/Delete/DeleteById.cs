using Application.Shared;
using Domain.Repositories;

namespace Application.Features.Postes.Delete
{
    public record DeletePosteByIdsCommand : IRequest<Result<Guid>>
    {
        public Guid UserId { get; set; }
        public Guid PosteId { get; init; }
    }

    public class DeletePosteByIdsCommandValidator : AbstractValidator<DeletePosteByIdsCommand>
    {
        public DeletePosteByIdsCommandValidator()
        {
            RuleFor(u => u.UserId).NotEmpty();
            RuleFor(p => p.PosteId).NotEmpty();
        }
    }

    public class DeletePosteByIdsHandler : IRequestHandler<DeletePosteByIdsCommand, Result<Guid>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPosteRepository _posteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePosteByIdsHandler(IUserRepository userRepository, IPosteRepository posteRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<Guid>> Handle(DeletePosteByIdsCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result.Fail("User not found");

            var deletedPoste = user.Postes!.FirstOrDefault(p => p.Id == request.PosteId);
            if (deletedPoste == null)
                return Result.Fail<Guid>("Post not found");

            await _posteRepository.DeleteFileFromCloudAsync(deletedPoste.Id.ToString());

            var response = await _posteRepository.DeleteRangeAsync(deletedPoste.Id);
            _userRepository.Update(user);
            await _unitOfWork.SaveCommitAsync();

            return Result.Ok(response[0]);
        }

    }
}
