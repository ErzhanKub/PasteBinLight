namespace Domain.IServices
{
    // Interface for a service that interacts with the cloud
    public interface IRecordCloudService
    {
        // Upload a text file to the cloud
        Task<string> UploadTextFileToCloudAsync(string key, string text);

        // Fetch a text file from the cloud
        Task<string> FetchTextFileFromCloudAsync(Uri url);

        // Delete a text file from the cloud
        Task DeleteTextFileFromCloudAsync(string key);

        // Update a text file in the cloud
        Task UpdateTextFileInCloudAsync(string key, string text);
    }
}
