using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Cache
{
    public class ChannelSummaryCache : AbstractRedisCaching<long, ChannelSummaryDTO>
    {
        private readonly ChannelRepo _channelRepo;
        public ChannelSummaryCache(IConnectionMultiplexer redis, ChannelRepo channelRepo) : base(redis)
        {
            _channelRepo = channelRepo;
        }

        public override string GetKey(long key)
        {
            return RedisConstant.GetKey(RedisConstant.CHANNEL_SUMMARY, key);
        }

        public override async Task<Dictionary<long, ChannelSummaryDTO>> Load(List<long> keys)
        {
            var channels = await _channelRepo.GetChannelByIds(keys);
            return channels.ToDictionary(c => c.Id, c => new ChannelSummaryDTO
            {
                Topic = c.Topic,
                Description = c.Description,
                ChannelImgUrl = c.ChannelImgUrl,
                ChannelImgBackground = c.ChannelImgBackground,
                ChannelOwnerId = c.UserId
            });
        }
    }
}
