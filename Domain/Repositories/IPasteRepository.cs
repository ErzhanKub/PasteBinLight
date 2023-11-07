using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories;

public interface IPasteRepository : IGenericRepository<Poste>
{
    public Task<string> UploadTextToCloudAsync(string objectKey, string text);
    public Task<string> GetTextFromCloudAsync(Uri url);
    public Task DeleteTextFromCloudAsync(string objectKey);
    public Task EditTextFromCloudeAsync(string objectKey, string text);
    public string GetEncodedGuid(Guid guid);
    public Guid GetDecodedGuid(string decodedBytes);
}