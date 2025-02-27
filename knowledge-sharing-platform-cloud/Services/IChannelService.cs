using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IChannelService
    {
        Task<CreateChannelResp> CreateChannel(CreateChannelReq channel);

        Task<ApiResult<JoinChannelResp>> JoinChannel(JoinChannelReq channel);
    }
}
