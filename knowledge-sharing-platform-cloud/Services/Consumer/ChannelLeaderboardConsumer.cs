using AWS.Messaging;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Services.Consumer
{
    public class ChannelLeaderboardConsumer : IMessageHandler<ChannelLeaderboardDTO>
    {
        private readonly ILogger<ChannelLeaderboardConsumer> _logger;
        
        private ChannelRepo _channelRepo;
        private ChannelLeaderboardCache _leaderboardCache;

        // Constructor to inject IWebsocketService
        public ChannelLeaderboardConsumer(ILogger<ChannelLeaderboardConsumer> logger, ChannelRepo channelRepo,ChannelLeaderboardCache leaderBoardCache)
        {  
            _logger = logger;
            _channelRepo = channelRepo;  
            _leaderboardCache = leaderBoardCache;
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
                await _leaderboardCache.UpdateLeaderboardIfNecessary(channelLeaderboardDTO.channelId, totalMemberCount);
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
