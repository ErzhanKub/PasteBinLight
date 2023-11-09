using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using Domain.IServices;

namespace Infrastructure.Services
{
    // Cloud service class that implements the IRecordCloudService interface
    public sealed class CloudService : IRecordCloudService
    {
        // AWS S3 bucket details
        public const string bucketName = "basketforfinalproject";
        public const string accessKey = "AKIARQAYJTSFMJZ3APWV";
        public const string secretKey = "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";

        // Delete a text file from the cloud
        public async Task DeleteTextFileFromCloudAsync(string keyName)
        {
            using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1);
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = keyName
            };
            await client.DeleteObjectAsync(deleteRequest);
        }

        // Edit a text file in the cloud
        public async Task UpdateTextFileInCloudAsync(string objectKey, string text)
        {
            using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1);
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                ContentBody = text
            };
            await client.PutObjectAsync(request);
        }

        // Get a text file from the cloud
        public async Task<string> FetchTextFileFromCloudAsync(Uri url)
        {
            using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1);

            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = url.AbsolutePath.Substring(1)
            };

            using var response = await client.GetObjectAsync(request);

            using var reader = new StreamReader(response.ResponseStream);

            string text = await reader.ReadToEndAsync();

            return text;
        }

        // Upload a text file to the cloud
        public async Task<string> UploadTextFileToCloudAsync(string objectKey, string text)
        {
            using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1);
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                ContentBody = text
            };
            await client.PutObjectAsync(request);

            var url = $"https://{bucketName}.s3.{RegionEndpoint.EUNorth1.SystemName}.amazonaws.com/{objectKey}";
            return url;
        }
    }
}
