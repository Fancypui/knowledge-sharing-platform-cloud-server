using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface ILikesService
    {
        Task<LikeDislikePostResp> LikeDislikePost(LikeDislikePostReq likeDislikePostReq);

        Task<IEnumerable<UsersWhoLikedPostListResp>> UsersWhoLikedPostList(UsersWhoLikedPostListReq usersWhoLikedPostListReq);
    }
}