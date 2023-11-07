namespace Domain.IServices;

public interface IPasteCloudService
{
    public Task<string> UploadTextToCloudAsync(string key, string text);
    public Task<string> GetTextFromCloudAsync(Uri url);
    public Task DeleteTextFromCloudAsync(string key);
    public Task EditTextFromCloudeAsync(string key, string text);
}
