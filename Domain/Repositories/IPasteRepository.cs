using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface IPasteRepository : IGenericRepository<Paste>
{
    public string GetEncodedGuid(Guid guid);
    public Guid GetDecodedGuid(string decodedBytes);
}