using Application.Shared;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Features.Postes.Create
{
    public record CreatePosteCommand : IRequest<Result<string>>
    {
        public Guid UserId { get; set; }
        public CreatePosteDto? Poste { get; init; }
    }

    public class CreatePosteCommandValidator : AbstractValidator<CreatePosteCommand>
    {
        public CreatePosteCommandValidator()
        {
            RuleFor(u => u.UserId).NotEmpty();
            RuleFor(p => p.Poste).NotNull();

            When(p => p.Poste != null, () =>
            {
                RuleFor(p => p.Poste!.Text).NotEmpty();
                RuleFor(p => p.Poste!.Title).Length(0, 200);
                RuleFor(p => p.Poste!.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
            });
        }
    }

    public class CreatePosteHandler : IRequestHandler<CreatePosteCommand, Result<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPosteRepository _posteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePosteHandler(IUserRepository userRepository, IPosteRepository posteRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _posteRepository = posteRepository ?? throw new ArgumentNullException(nameof(posteRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Result<string>> Handle(CreatePosteCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);

            if (user == null)
                return Result.Fail("User not found");

            var posteId = Guid.NewGuid();
            var urlCloud = await _posteRepository.UploadTextToCloudAsync(posteId.ToString(), request.Poste!.Text);
            var poste = new Poste
            {
                Id = posteId,
                DateCreated = DateTime.Now,
                DeadLine = request.Poste.DeadLine,
                IsPrivate = request.Poste.IsPrivate,
                Title = request.Poste.Title,
                Url = new Uri(urlCloud),
                User = user,
                UserId = user.Id,
            };

            user.Postes!.Add(poste);

            var guid = await _posteRepository.CreateAsync(poste);
            _userRepository.Update(user);
            await _unitOfWork.SaveCommitAsync();

            var encodedGuid = _posteRepository.GetEncodedGuid(guid);

            return Result.Ok(encodedGuid);
        }
    }
}
