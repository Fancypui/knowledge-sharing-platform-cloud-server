using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Services.Consumer;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Cache
{
    public class ChannelLeaderboardCache
    {
        private readonly IDatabase _redisDB;
        private readonly string _luaScript;
        private readonly int LeaderboardSize = 500;
        private readonly string CHANNEL_LEADERBOARD_LOCK_KEY = "CHANNEL_LEADERBOARD_LOCK_KEY";
        private readonly int CHANNEL_LEADERBOARD_LOCK_EXPIRY = 10;
        private readonly ChannelRepo _channelRepo;

        public ChannelLeaderboardCache(IConnectionMultiplexer redis, ChannelRepo channelRepo)
        {
            _redisDB = redis.GetDatabase();
            string luaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Cache", "ChannelLeaderboardUpdate.lua");
            _luaScript = File.ReadAllText(luaFilePath);
 


            _channelRepo = channelRepo;
        }


        public async Task UpdateLeaderboardIfNecessary(long channelId, int newMemberCount)
        {
            await _redisDB.ScriptEvaluateAsync(_luaScript, new RedisKey[] { RedisConstant.CHANNEL_LEADERBOARD }, 
                new RedisValue[] { channelId, newMemberCount});
        }

        public async Task<IEnumerable<(long Id, int TotalMember)>> GetLeaderboardPage(long? cursor, int pageSize)
        {
            /**
             * if cursor highest than leaderboard size, return empty list
             */
            if (cursor >= LeaderboardSize)
            {
                return Enumerable.Empty<(long, int)>();
            }
            if (cursor == null)
            {
                cursor = 0;
            }
            /*
             * get leaderboard channel from redis
             */
            var leaderboardEntries = await _redisDB.SortedSetRangeByRankWithScoresAsync(
                                    RedisConstant.CHANNEL_LEADERBOARD,
                                    start: (int)cursor,
                                    stop: (int)cursor + pageSize - 1,
                                    order: Order.Descending
                                );
            if (leaderboardEntries !=null &&leaderboardEntries.Length > 0)
            {
                return leaderboardEntries.Select(entry => ((long)entry.Element, (int)entry.Score));
            }
            var currentChannelCount = await _channelRepo.GetChannelCountUpTo500();
            /**
             * if cursor size is bigger than current size of channels in db, immediately return
             */
            if (currentChannelCount <= cursor)
            {
                return Enumerable.Empty<(long, int)>();
            }
            /**
             * if empty list obtained from redis, need to reconstruct the key 
             */
            RedisValue token = Environment.MachineName;
            if (_redisDB.LockTake(CHANNEL_LEADERBOARD_LOCK_KEY,token,TimeSpan.FromSeconds(CHANNEL_LEADERBOARD_LOCK_EXPIRY)))
            {
                try
                {
                    /**
                     * check again, double checking mechanism
                     */
                    leaderboardEntries = await _redisDB.SortedSetRangeByRankWithScoresAsync(
                                    RedisConstant.CHANNEL_LEADERBOARD,
                                    start: (int)cursor,
                                    stop: (int)cursor + pageSize - 1,
                                    order: Order.Descending
                                );
                    if (leaderboardEntries != null && leaderboardEntries.Length > 0)
                    {
                        return leaderboardEntries.Select(entry => ((long)entry.Element, (int)entry.Score));
                    }
                    /**
                     * load 500 records
                     */
                    var leaderboardList = await _channelRepo.GetTop500Channels();
                    /**
                     * store records into cache
                     */
                    if (leaderboardList != null && leaderboardList.Any())
                    {
                        var sortedSetEntries = leaderboardList
                            .Select(channel => new SortedSetEntry(channel.Id, channel.TotalMember))
                            .ToArray();
                        /**
                         * remove existing records, and replace them with new one
                         */
                        await _redisDB.KeyDeleteAsync(RedisConstant.CHANNEL_LEADERBOARD);
                        await _redisDB.SortedSetAddAsync(RedisConstant.CHANNEL_LEADERBOARD, sortedSetEntries);
                    }
                }
                finally{
                    _redisDB.LockRelease(CHANNEL_LEADERBOARD_LOCK_KEY,token);
                }
            }
            leaderboardEntries = await _redisDB.SortedSetRangeByRankWithScoresAsync(
                                    RedisConstant.CHANNEL_LEADERBOARD,
                                    start: (int)cursor,
                                    stop: (int)cursor + pageSize - 1,
                                    order: Order.Descending
                                );
            if (leaderboardEntries != null && leaderboardEntries.Length > 0)
            {
                return leaderboardEntries.Select(entry => ((long)entry.Element, (int)entry.Score));
            }
            return Enumerable.Empty<(long, int)>();



        }
    }
}
