using Amazon.S3.Model;
using Amazon.S3;
using Domain.Entities;
using Domain.Repositories;
using Amazon;
using Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PosteRepository : IPosteRepository
    {
        public const string bucketName = "basketforfinalproject";
        public const string accessKey = "AKIARQAYJTSFMJZ3APWV";
        public const string secretKey = "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";

        private readonly AppDbContext _dbcontext;

        public PosteRepository(AppDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Guid> CreateAsync(Poste entity)
        {
            await _dbcontext.Postes.AddAsync(entity);
            return entity.Id;
        }

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

        public Task<Guid[]> DeleteRangeAsync(params Guid[] ids)
        {
            var posteToDelete = _dbcontext.Postes.Where(p => ids.Contains(p.Id));
            _dbcontext.Postes.RemoveRange(posteToDelete);
            return Task.FromResult(ids);
        }

        public Task<List<Poste>> GetAllAsync()
        {
            return _dbcontext.Postes.AsNoTracking().ToListAsync();
        }

        public async Task<Poste?> GetByIdAsync(Guid id)
        {
            var poste = await _dbcontext.Postes.FirstOrDefaultAsync(p => p.Id == id);
            return poste;
        }

        public Guid GetDecodedGuid(string decodedBytes)
        {
            byte[] decoded = Convert.FromBase64String(decodedBytes);
            var guid = new Guid(decoded);
            return guid;
        }

        public string GetEncodedGuid(Guid guid)
        {
            byte[] guidBytes = guid.ToByteArray();
            string base64Guid = Convert.ToBase64String(guidBytes);
            return base64Guid;
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

        public void Update(Poste entity)
        {
            _dbcontext.Postes.Update(entity);
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

            var url = "https://basketforfinalproject.s3.eu-north-1.amazonaws.com/" + objectKey;
            return url;
        }
    }
}