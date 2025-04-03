using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface ILikesService
    {
        Task<LikeDislikePostResp> LikeDislikePost(LikeDislikePostReq likeDislikePostReq,long uid);

        Task<CursorBasedResp<UsersWhoLikedPostListResp>> UsersWhoLikedPostList(UsersWhoLikedPostListReq usersWhoLikedPostListReq);
    }
}