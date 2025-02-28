using AWS.Messaging;
using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Services.Consumer
{
    public class ChannelLeaderboardConsumer : IMessageHandler<ChannelLeaderboardDTO>
    {
        private readonly ILogger<PushNotificationConsumer> _logger;
        private readonly IDatabase _redisDB;
        private readonly string _luaScript;
        
        private ChannelRepo _channelRepo;

        // Constructor to inject IWebsocketService
        public ChannelLeaderboardConsumer(IConnectionMultiplexer redis, ILogger<PushNotificationConsumer> logger, ChannelRepo channelRepo)
        {
            _redisDB = redis.GetDatabase();
            _logger = logger;
            _channelRepo = channelRepo;
            string luaFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "ChannelLeaderboardUpdate.lua");
            if (File.Exists(luaFilePath)) {
                _luaScript = File.ReadAllText(luaFilePath);
            }
            else
            {
                throw new FileNotFoundException("Lua script file not found: " + luaFilePath);
            }
            
        }
        public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<ChannelLeaderboardDTO> messageEnvelope, CancellationToken token = default)
        {
            // Validate message envelope
            if (messageEnvelope == null || messageEnvelope.Message == null)
            {
                _logger.LogInformation("Message Envelope or Message is null");
                return MessageProcessStatus.Failed();
            }
            var channelLeaderboardDTO = messageEnvelope.Message;
            try
            {
                // Get total members of channel from db
                var totalMemberCount = await _channelRepo.TotalMemberByChannelId(channelLeaderboardDTO.channelId);

                // Execute the Lua script in Redis to update the channel leaderboad
                await _redisDB.ScriptEvaluateAsync(_luaScript, new RedisKey[] { RedisConstant.CHANNEL_LEADERBOARD }, new RedisValue[] { channelLeaderboardDTO.channelId, totalMemberCount });

                return MessageProcessStatus.Success();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error processing channel leaderboard message");
                return MessageProcessStatus.Failed();
            }
        }
    }
}
