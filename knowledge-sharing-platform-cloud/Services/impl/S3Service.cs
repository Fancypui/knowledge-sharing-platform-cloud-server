using Amazon.S3;
using Amazon.S3.Model;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;

        private static HttpClient _httpClient = new HttpClient();

        public S3Service(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<HttpResponseMessage> UploadFileAsync(string filePath, string url)
        {
            using var streamContent = new StreamContent(
            new FileStream(filePath, FileMode.Open, FileAccess.Read));

            var response = await _httpClient.PutAsync(url, streamContent);

            return response;
        }

        public async Task<GetS3PresignedUrlResp> GeneratePresignedUrl(GetS3PresignedUrlReq getS3PresignedUrlReq)
        {
            string bucketName = "ddac-assignment-s3-post";

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists)
            {
                throw new BusinessException("Failed to generate S3 presigned url. Bucket does not exists");
            }

            string objectKey = getS3PresignedUrlReq.objectKey;

            GetPreSignedUrlRequest request = new()
            {
                BucketName = bucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            string presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

            GetS3PresignedUrlResp response = new()
            {
                S3PresignedUrl = presignedUrl,
            };

            return response;
        }
    }
}
