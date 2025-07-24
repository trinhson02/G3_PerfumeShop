using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

namespace G3_PerfumeShop.Service
{
    public class S3Service
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;

        public S3Service(IConfiguration configuration)
        {
            _configuration = configuration;
            _s3Client = new AmazonS3Client(
                _configuration["AWS:AccessKey"],
                _configuration["AWS:SecretKey"],
                new AmazonS3Config
                {
                    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_configuration["AWS:Region"]),
                    ServiceURL = _configuration["AWS:ServiceURL"]
                });
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty", nameof(file));
            }

            var bucketName = _configuration["AWS:BucketName"];
            var fileExtension = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(fileExtension))
            {
                throw new ArgumentException("File must have an extension", nameof(file));
            }

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = Guid.NewGuid().ToString() + fileExtension,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead
            };

            try
            {
                using (var transferUtility = new TransferUtility(_s3Client))
                {
                    await transferUtility.UploadAsync(uploadRequest);
                }
                return $"{_configuration["AWS:ServiceURL"]}/{bucketName}/{uploadRequest.Key}";
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"S3 error: {e.Message}");
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                throw; 
            }
        }

    }
}
