namespace Application.Features.Postes.Update
{
    public class UpdatePosteByIdCommand : IRequest<Result<PosteDto>>
    {
        public Guid UserId { get; set; }
        public Guid PosteId { get; init; }
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
        private readonly IPosteRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePosteByIdHandler(IPosteRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public Task<Result<PosteDto>> Handle(UpdatePosteByIdCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
