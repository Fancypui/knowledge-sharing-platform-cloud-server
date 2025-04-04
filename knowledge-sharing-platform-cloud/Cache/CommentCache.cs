using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Cache
{
    public class CommentCache : AbstractRedisCaching<long, Comment>
    {
        private readonly CommentRepo _commentRepo;
        public CommentCache(IConnectionMultiplexer redis,CommentRepo commentRepo) : base(redis)
        {
            _commentRepo = commentRepo;
        }

        public override string GetKey(long key)
        {
            return RedisConstant.GetKey(RedisConstant.COMMENT_DETAIL, key);
        }

        public override async Task<Dictionary<long, Comment>> Load(List<long> keys)
        {
            var comments = await _commentRepo.GetCommentByIds(keys);
            return comments.ToDictionary(c => c.Id, c => c);
        }
    }
}
