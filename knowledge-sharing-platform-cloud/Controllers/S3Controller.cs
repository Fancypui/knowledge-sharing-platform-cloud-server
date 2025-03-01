using Amazon.S3;
using Amazon.S3.Model;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Services.impl;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("s3")]
    [ApiController]
    public class S3Controller : Controller
    {
        private readonly S3Service _s3Service;

        public S3Controller(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        //[HttpPost]
        //public async Task<IActionResult> CreateBucketAsync(string bucketName)
        //{
        //    await _s3Client.PutBucketAsync(bucketName);
        //    return Created("buckets", "It works cibaii");
        //}

        //[HttpPost("upload")]
        //public async Task<IActionResult> UploadFile(IFormFile file, string bucketName, string? prefix)
        //{
        //    var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName);
        //    if (!bucketExists) return NotFound("Bucket does not exists");

        //    var request = new PutObjectRequest
        //    {
        //        BucketName = bucketName,
        //        Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
        //        InputStream = file.OpenReadStream(),
        //    };

        //    request.Metadata.Add("Content-Type", file.ContentType);
        //    await _s3Client.PutObjectAsync(request);

        //    return Ok("file uploaded");
        //}

        [HttpGet("url")]
        public async Task<ApiResult<GetS3PresignedUrlResp>> GetS3PresignedUrl([FromQuery] GetS3PresignedUrlReq getS3PresignedUrlReq)
        {
            GetS3PresignedUrlResp getS3PresignedUrlResp = await _s3Service.GeneratePresignedUrl(getS3PresignedUrlReq);

            return ApiResult<GetS3PresignedUrlResp>.ServiceSucess(getS3PresignedUrlResp);
        }

        [HttpPost("testing")]
        public async Task<HttpResponseMessage> UploadFileAsync(string filePath, string url)
        {
            var message = await _s3Service.UploadFileAsync(filePath, url);

            return message;
        }
    }
}
