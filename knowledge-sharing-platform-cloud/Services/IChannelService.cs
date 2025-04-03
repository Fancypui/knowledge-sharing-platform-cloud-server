using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.ChannelReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IChannelService
    {
        Task<CreateChannelResp> CreateChannel(CreateChannelReq channel);

        Task<JoinChannelResp> JoinChannel(JoinChannelReq channel);

        Task<String> JoinChannelSuccess(string userId, string channelId, decimal feePaid);
                                              
        Task<GetChannelSummaryResp> GetChannelSummary(GetChannelSummaryReq getChannelSummaryReq,long uid);

        Task<GetChannelOwnerSummaryResp> GetChannelOwnerSummary(GetChannelOwnerSummaryReq getChannelOwnerSummaryReq);


        Task<CursorBasedResp<ChannelLeaderboardListResp>> ChannelLeaderboardList(CursorBaseReq request);

        Task<IEnumerable<SearchChannelByTopicResp>> SearchChannelByTopic(SearchChannelByTopicReq searchChannelByTopicReq);
    }
}
