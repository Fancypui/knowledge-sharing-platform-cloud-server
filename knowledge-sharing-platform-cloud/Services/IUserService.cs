using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IUserService
    {
        Task<CreateUserResp> CreateUser(CreateUserReq createUserReq);

        Task<IEnumerable<UserJoinedChannelListResp>> UserJoinedChannelList(UserJoinedChannelListReq userJoinedChannelListReq);

        Task<IEnumerable<UserManagedChannelListResp>> UserManagedChannelList(UserManagedChannelListReq userManageChannelListReq);
    }
}