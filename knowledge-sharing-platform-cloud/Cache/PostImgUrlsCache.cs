using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Services.impl;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Cache
{
    public class PostImgUrlsCache : AbstractRedisCaching<long, PostImageUrlDTO>
    {
        private readonly PostRepo _postRepo;
        private readonly S3Service _s3Service;
        public PostImgUrlsCache(IConnectionMultiplexer redis, PostRepo postRepo, S3Service s3Service) : base(redis)
        {
            _postRepo = postRepo;
            _s3Service = s3Service;
        }
        public override string GetKey(long key)
        {
            return RedisConstant.GetKey(RedisConstant.POST_IMAGE_PRESIGNED_URLS, key);
        }

        public override async Task<Dictionary<long, PostImageUrlDTO>> Load(List<long> keys)
        {
            List<Post> postWithImgUrl = await _postRepo.GetPostImgUrlsByIds(keys);

            var result = await Task.WhenAll(postWithImgUrl.Select(async post =>
            {
                string[] postImageUrlList = JsonConvert.DeserializeObject<string[]>(post.PostImgUrl);

                string[] s3PresignedUrlList = await Task.WhenAll(postImageUrlList.Select(async imageUrl =>
                {
                    GetS3PresignedUrlReq request = new()
                    {
                        objectKey = imageUrl
                    };

                    var s3Response = await _s3Service.GeneratePresignedUrlToRetrieve(request);

                    return s3Response.S3PresignedUrl;
                }));

                PostImageUrlDTO postImageUrlDTO = new()
                {
                    PostId = post.Id,
                    ImageUrl = s3PresignedUrlList,
                };

                return postImageUrlDTO;
            }));

            return result.ToDictionary(c=>c.PostId, c=>c);
        }
    }
}
