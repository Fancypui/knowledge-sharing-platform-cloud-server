using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.ChannelReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IChannelService
    {
        Task<CreateChannelResp> CreateChannel(CreateChannelReq channel);

        Task<JoinChannelResp> JoinChannel(JoinChannelReq channel);

        Task<String> JoinChannelSuccess(string userId, string channelId, decimal feePaid);
                                              
        Task<GetChannelSummaryResp> GetChannelSummary(GetChannelSummaryReq getChannelSummaryReq);

        Task<GetChannelOwnerSummaryResp> GetChannelOwnerSummary(GetChannelOwnerSummaryReq getChannelOwnerSummaryReq);

        Task<IEnumerable<string>> SearchChannelByTopic(string channelTopic);

        Task<CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>>> ChannelLeaderboardList(CursorBaseReq request);

        
        Task<IEnumerable<SearchChannelByTopicResp>> SearchChannelByTopic(SearchChannelByTopicReq searchChannelByTopicReq);
    }
}
