using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class LikesServiceImpl : ILikesService
    {
        private readonly LikesRepo _likesRepo;

        public LikesServiceImpl(LikesRepo likesRepo)
        {
            _likesRepo = likesRepo;
        }

        public async Task<LikeDislikePostResp> LikeDislikePost(LikeDislikePostReq likeDislikePostReq)
        {
            bool changedStatus = await _likesRepo.ChangeLikeStatus(likeDislikePostReq.UserId, likeDislikePostReq.PostId, likeDislikePostReq.IsLiked);

            if (!changedStatus)
            {
                throw new BusinessException("Failed to change like status");
            }

            LikeDislikePostResp response = new()
            {
                LastLikeStatus = likeDislikePostReq.IsLiked
            };

            return response;
        }
    }
}
