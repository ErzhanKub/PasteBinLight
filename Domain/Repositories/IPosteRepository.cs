using Domain.Entities;
using Domain.Shared;

namespace Domain.Repositories
{
    public interface IPosteRepository : IRepository<Poste>
    {
        public Task<string> UploadTextToCloudAsync(string objectKey, string text);
        public Task<string> GetTextFromCloudAsync(Uri url);
        public string GetEncodedGuid(Guid guid);
        public Guid GetDecodedGuid(string decodedBytes);
    }
}