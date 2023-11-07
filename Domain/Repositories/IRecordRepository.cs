using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface IRecordRepository : IGenericRepository<Record>
{
    public string GetEncodedGuid(Guid guid);
    public Guid GetDecodedGuid(string decodedBytes);
}