using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IUserService
    {
        Task<CreateUserResp> CreateUser(CreateUserReq createUserReq);

        Task<IEnumerable<UserJoinedChannelListResp>> UserJoinedChannelList(long uid);

        Task<IEnumerable<UserManagedChannelListResp>> UserManagedChannelList(long uid);

        Task<UserRegisterResp> userRegistration(UserRegisterReq request);

        Task<UserLogInResp> userLogIn(UserLogInReq request);

        Task<UserInfoResp> GetUserInfo(long uid);

        Task SaveUserWebPushSubcription(SaveUserWebPushSubscription request, long uid);

    }
}