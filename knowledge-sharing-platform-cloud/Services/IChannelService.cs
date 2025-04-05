
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

        Task<JoinChannelSuccessResp> JoinChannelSuccess(string userId, string channelId, decimal feePaid);

        Task<JoinChannelFailResp> JoinChannelFail(string channelId);

        Task<GetChannelSummaryResp> GetChannelSummary(GetChannelSummaryReq getChannelSummaryReq,long uid);

        Task<GetChannelOwnerSummaryResp> GetChannelOwnerSummary(GetChannelOwnerSummaryReq getChannelOwnerSummaryReq);

        Task<CursorBasedResp<ChannelLeaderboardListResp>> ChannelLeaderboardList(ChannelLeaderboardListReq request);

        Task<CursorBasedResp<SearchChannelByTopicResp>> SearchChannelByTopic(SearchChannelByTopicReq searchChannelByTopicReq);

        Task<CheckUserJoinChannelResp> CheckUserJoinChannel(CheckUserJoinChannel request);
    }
}
