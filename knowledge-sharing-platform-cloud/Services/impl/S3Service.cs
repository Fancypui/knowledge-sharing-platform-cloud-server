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
        private readonly string _resizeBucket;

        private readonly string _unresizeBucket;

        public S3Service(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _unresizeBucket = config["AWS:S3UnoptimizedBucket"];
            _resizeBucket = config["AWS:S3OptimizedBucket"];
        }

        public async Task<HttpResponseMessage> UploadFileAsync(string filePath, string url)
        {
            using var streamContent = new StreamContent(
            new FileStream(filePath, FileMode.Open, FileAccess.Read));

            var response = await _httpClient.PutAsync(url, streamContent);

            return response;
        }

        public async Task<GetS3PresignedUrlResp> GeneratePresignedUrlToUpload(IEnumerable<GetS3PresignedUrlReq> getS3PresignedUrlReq)
        {
            string bucketName = _unresizeBucket;

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists)
            {
                throw new BusinessException("Failed to generate S3 presigned url. Bucket does not exists");
            }

            string[] s3PresignedUploadUrls = await Task.WhenAll(getS3PresignedUrlReq.Select(async file =>
            {
                GetPreSignedUrlRequest request = new()
                {
                    BucketName = bucketName,
                    Key = file.ObjectKey,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    ContentType = file.FileType
                };

                string presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

                return presignedUrl;
            }));

            GetS3PresignedUrlResp response = new()
            {
                S3PresignedUrls = s3PresignedUploadUrls,
            };

            return response;
        }

        public async Task<GetS3PresignedUrlResp> GeneratePresignedUrlToRetrieve(IEnumerable<GetS3PresignedUrlReq> getS3PresignedUrlReq)
        {
            string bucketName = _resizeBucket;

            var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
            if (!bucketExists)
            {
                throw new BusinessException("Failed to generate S3 presigned url. Bucket does not exists");
            }

            string[] s3PresignedRetrieveUrls = await Task.WhenAll(getS3PresignedUrlReq.Select(async file =>
            {
                GetPreSignedUrlRequest request = new()
                {
                    BucketName = bucketName,
                    Key = file.ObjectKey,
                    Verb = HttpVerb.GET,
                    Expires = DateTime.UtcNow.AddMinutes(135)
                };

                string presignedUrl = await _s3Client.GetPreSignedURLAsync(request);

                return presignedUrl;
            }));



            GetS3PresignedUrlResp response = new()
            {
                S3PresignedUrls = s3PresignedRetrieveUrls,
            };

            return response;

        }
    }
}
