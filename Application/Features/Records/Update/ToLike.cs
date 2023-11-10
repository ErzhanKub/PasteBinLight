using Application.Features.Records.Update;

namespace Application.Features.Records.Update;

public record ToLike : IRequest<Result<RecordDto>>
{
    public Guid Id { get; init; }
    public ToLikeDto? Data { get; init; }
}
public record ToLikeDto 
{
    public long Likes { get; init; }
}

public sealed class ToLikeComamndHandler : IRequestHandler<ToLike, Result<RecordDto>>
{
    private readonly IRecordRepository _recordRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ToLikeComamndHandler(IRecordRepository recordRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _recordRepository = recordRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public Task<Result<RecordDto>> Handle(ToLike request, CancellationToken cancellationToken)
    {

    }
}


