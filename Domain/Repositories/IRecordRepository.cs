using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

// Record repository interface that extends the IGenericRepository interface
public interface IRecordRepository : IGenericRepository<Record>
{
    // Encode a GUID to a base64 string
    string EncodeGuidToBase64(Guid guid);

    // Decode a GUID from a base64 string
    Guid DecodeGuidFromBase64(string decodedBytes);

    // Fetch the top 100 records sorted by likes
    Task<IReadOnlyList<Record>> FetchTop100RecordsByLikesAsync(CancellationToken cancellationToken);

    // Find records by their title
    Task<IReadOnlyList<Record>> FindRecordsByTitleAsync(string title, CancellationToken cancellationToken);
    Task<List<Record>> GetAllRecords(CancellationToken cancellationToken);
}
