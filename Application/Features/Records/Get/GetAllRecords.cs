using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Records.Get
{
    public record GetAllRecordsRequest : IRequest<Result<List<AllRecordsDto>>> { }

    public sealed class GetAllRecordsValidator: AbstractValidator<GetAllRecordsRequest>
    {
        public GetAllRecordsValidator() { }

    }

    public sealed class GetAllRecordsHandler : IRequestHandler<GetAllRecordsRequest, Result<List<AllRecordsDto>>>
    {
        private readonly IRecordRepository _recordRepository;

        public GetAllRecordsHandler(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task<Result<List<AllRecordsDto>>> Handle(GetAllRecordsRequest request, CancellationToken cancellationToken)
        {
            var records = await _recordRepository.GetAllRecords(cancellationToken);
            var publicRecords = records.Where(r => r.IsPrivate == false).ToList();
            var response = publicRecords.Select(r => new AllRecordsDto
            {
                Id = r.Id,
                DateCreated = r.DateCreated,
                DeadLine = r.DeadLine,
                DisLikes = r.DisLikes,
                Likes = r.Likes,
                Title = r.Title,
                Url = r.Url,
            }).ToList();

            return Result.Ok(response);
        }
    }
}
