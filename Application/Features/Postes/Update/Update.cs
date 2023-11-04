using Application.Contracts;
using Application.Shared;
using Domain.Repositories;

namespace Application.Features.Postes.Update
{
    public class UpdatePosteByIdCommand : IRequest<Result<PosteDto>>
    {
        public Guid PosteId { get; init; }
        public Guid UserId { get; set; }
        public string? Text { get; init; }
        public string? Title { get; init; }
        public bool IsPrivate { get; init; }
        public DateTime DeadLine { get; init; }
    }

    public class UpdatePosteByIdvalidator : AbstractValidator<UpdatePosteByIdCommand>
    {
        public UpdatePosteByIdvalidator()
        {
            RuleFor(p => p.PosteId).NotEmpty();
            RuleFor(p => p.Title).Length(1, 200);
            RuleFor(p => p.DeadLine).GreaterThanOrEqualTo(DateTime.Now);
        }
    }

    public class UpdatePosteByIdHandler : IRequestHandler<UpdatePosteByIdCommand, Result<PosteDto>>
    {
        private readonly IPosteRepository _posteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UpdatePosteByIdHandler(IPosteRepository repository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _posteRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _userRepository = userRepository;
        }

        public async Task<Result<PosteDto>> Handle(UpdatePosteByIdCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user is null)
                return Result.Fail("User is not found");

            var poste = user.Postes.FirstOrDefault(p => p.Id == request.PosteId);

            if (poste is null)
                return Result.Fail("Poste is not found");

            if (request.Title is not null)
                poste.Title = request.Title;

            poste.DeadLine = request.DeadLine;
            poste.IsPrivate = request.IsPrivate;

            await _posteRepository.EditTextFromCloudeAsync(poste.Id.ToString(), request.Text ?? string.Empty);

            _posteRepository.Update(poste);
            await _unitOfWork.SaveCommitAsync();

            var response = new PosteDto
            {
                Id = poste.Id,
                DeadLine = request.DeadLine,
                Text = request.Text ?? string.Empty,
                Title = request.Title,
                IsPrivate = request.IsPrivate,
            };

            return Result.Ok(response);
        }
    }
}
