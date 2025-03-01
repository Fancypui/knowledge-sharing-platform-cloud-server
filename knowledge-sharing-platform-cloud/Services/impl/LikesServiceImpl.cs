using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Likes;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CommentResp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class LikesServiceImpl : ILikesService
    {
        private readonly LikesRepo _likesRepo;
        private readonly UserRepo _userRepo;

        public LikesServiceImpl(LikesRepo likesRepo, UserRepo userRepo)
        {
            _likesRepo = likesRepo;
            _userRepo = userRepo;
        }

        public async Task<LikeDislikePostResp> LikeDislikePost(LikeDislikePostReq likeDislikePostReq)
        {
            Likes existingLikeRecord = await _likesRepo.FindLikesByUserIdAndPostIdAsync(likeDislikePostReq.UserId, likeDislikePostReq.PostId);

            if (existingLikeRecord == null)
            {
                Likes like = new()
                {
                    UserId = likeDislikePostReq.UserId,
                    PostId = likeDislikePostReq.PostId,
                    LikeStatus = likeDislikePostReq.IsLiked
                };

                Likes newLikeRecord = await _likesRepo.CreateLikesAsync(like);
            }
            else
            {
                bool changedStatus = await _likesRepo.ChangeLikeStatus(existingLikeRecord.Id, likeDislikePostReq.IsLiked);

                if (!changedStatus)
                {
                    throw new BusinessException("Failed to change like status");
                }
            }

            LikeDislikePostResp response = new()
            {
                LastLikeStatus = likeDislikePostReq.IsLiked
            };

            return response;
        }

        public async Task<IEnumerable<UsersWhoLikedPostListResp>> UsersWhoLikedPostList(UsersWhoLikedPostListReq usersWhoLikedPostListReq)
        {
            long? cursor = null;
            if (!usersWhoLikedPostListReq.IsFirstPage() && long.TryParse(usersWhoLikedPostListReq.Cursor, out var parsedCursor))
            {
                cursor = parsedCursor;
            }

            IEnumerable<Likes> likeList = await _likesRepo.GetPaginatedLikes(usersWhoLikedPostListReq.PostId, cursor, usersWhoLikedPostListReq.PageSize);

            List<long> userIdsWhoLikePostList = likeList.Select(l => l.UserId).ToList();

            IEnumerable<User> userList = await _userRepo.UserListByIds(userIdsWhoLikePostList);

            IEnumerable<UsersWhoLikedPostListResp> response = userList.Select(user =>
            {
                return new UsersWhoLikedPostListResp
                {
                    UserId = user.Id,
                    Username = user.Username,
                    ProfileUrl = user.ProfileUrl
                };
            });

            return response;
        }
    }
}
