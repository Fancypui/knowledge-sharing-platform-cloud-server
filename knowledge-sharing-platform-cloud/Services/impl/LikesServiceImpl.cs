using System.Linq;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CommentResp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class LikesServiceImpl : ILikesService
    {
        private readonly StackExchange.Redis.IDatabase _redisDB;
        private readonly LikesRepo _likesRepo;
        //private readonly UserRepo _userRepo;
        private readonly UserCache _userCache;
        private readonly PostRepo _postRepo;

        public LikesServiceImpl(LikesRepo likesRepo, UserCache userCache, IConnectionMultiplexer redis,PostRepo postRepo)
        {
            _redisDB = redis.GetDatabase();
            _likesRepo = likesRepo;
            _userCache= userCache;  
            _postRepo = postRepo;
        }

        public async Task<LikeDislikePostResp> LikeDislikePost(LikeDislikePostReq likeDislikePostReq, long uid)
        {
            if(likeDislikePostReq.PostId == null)
            {
                throw new BusinessException("Post not found");
            }
            var post = await _postRepo.GetPostById(likeDislikePostReq.PostId);
            if (post == null)
            {
                throw new BusinessException("Post not found");
            }

            RedisValue token = Environment.MachineName;
            string postLikeLockKey = RedisConstant.GetKey(RedisConstant.LIKE_POST_KEY,likeDislikePostReq.PostId,uid);
            if (_redisDB.LockTake(postLikeLockKey, token, TimeSpan.FromSeconds(20)))
            {
                try
                {
                    Likes? existingLikeRecord = await _likesRepo.FindLikesByUserIdAndPostIdAsync(uid, likeDislikePostReq.PostId);

                    if (existingLikeRecord == null)
                    {
                        Likes like = new()
                        {
                            UserId = uid,
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
                }
                finally
                {
                    _redisDB.LockRelease(postLikeLockKey, token);
                }
            }

            LikeDislikePostResp response = new()
            {
                LastLikeStatus = likeDislikePostReq.IsLiked
            };

            return response;
        }

        public async Task<CursorBasedResp<UsersWhoLikedPostListResp>> UsersWhoLikedPostList(UsersWhoLikedPostListReq usersWhoLikedPostListReq)
        {
            long? cursor = null;
            if (!usersWhoLikedPostListReq.IsFirstPage() && long.TryParse(usersWhoLikedPostListReq.Cursor, out var parsedCursor))
            {
                cursor = parsedCursor;
            }

            IEnumerable<Likes> likeList = await _likesRepo.GetPaginatedLikes(usersWhoLikedPostListReq.PostId, cursor, usersWhoLikedPostListReq.PageSize);

            List<long> userIdsWhoLikePostList = likeList.Select(l => l.UserId).ToList();

            var userMap  = await _userCache.GetBatch(userIdsWhoLikePostList);

            var listData = likeList.Select(like =>
            {
                var user = userMap.GetValueOrDefault(like.UserId, null);
                return new UsersWhoLikedPostListResp
                {
                    UserId = user?.Id, // `user?.Id` ensures it's null-safe
                    Username = user?.Username,
                    ProfileUrl = user?.ProfileUrl,
                    LikeId = like.Id
                };
            }).ToList();

            long? cursorId = listData.Any() ? listData.Min(like=>like.LikeId) : null;
            return CursorBasedResp<UsersWhoLikedPostListResp>.Init(listData, cursorId, listData.Count() < usersWhoLikedPostListReq.PageSize);
        }
    }
}
