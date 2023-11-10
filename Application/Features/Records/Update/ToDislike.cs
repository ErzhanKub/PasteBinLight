namespace Application.Features.Records.Update;

public record ToDislike : IRequest<Result<RecordDto>>
{
    public Guid Id { get; init; }
    public ToDislikeDto? Data { get; init; }
}
public record ToDislikeDto
{
    public long Dislikes { get; init; }
}

public sealed class ToDislikeComamndHandler : IRequestHandler<ToDislike, Result<RecordDto>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToDislikeComamndHandler(IRecordRepository recordRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _recordRepository = recordRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public Task<Result<RecordDto>> Handle(ToDislike request, CancellationToken cancellationToken)
    {
       
    }
}



