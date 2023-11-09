using Amazon.S3.Model;
using Amazon.S3;
using Amazon;
using Domain.IServices;

namespace Infrastructure.Services;

public class RecordCloudService : IRecordCloudService
{
    public const string bucketName = "basketforfinalproject";
    public const string accessKey = "AKIARQAYJTSFMJZ3APWV";
    public const string secretKey = "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";

    public async Task DeleteTextFromCloudAsync(string keyName)
    {
        using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1);
        var deletedRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = keyName
        };
        await client.DeleteObjectAsync(deletedRequest);
    }

    public async Task EditTextFromCloudeAsync(string objectKey, string text)
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

    public async Task<string> GetTextFromCloudAsync(Uri url)
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

    public async Task<string> UploadTextToCloudAsync(string objectKey, string text)
    {
        using var client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUNorth1);
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = objectKey,
            ContentBody = text
        };
        await client.PutObjectAsync(request);

        //var urlRequest = new GetPreSignedUrlRequest
        //{
        //    BucketName = bucketName,
        //    Key = objectKey,
        //    //Expires = DateTime.UtcNow.AddDays(7),
        //};
        //var url = client.GetPreSignedURL(urlRequest);

        var url = $"https://{bucketName}.s3.{RegionEndpoint.EUNorth1.SystemName}.amazonaws.com/{objectKey}";
        return url;
    }
}
