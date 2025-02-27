using System.Reflection;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class CommentServiceImpl : ICommentSerivce
    {
        /**
         * comment repository instance (dao)
         */
        private readonly CommentRepo _commentRepo;

        private readonly UserRepo _userRepo;
        private readonly CommentCache _commentCache;
        private readonly UserCache _userCache;  

        public CommentServiceImpl(CommentRepo commentRepo, UserRepo userRepo,CommentCache commentCache, UserCache userCache)
        {
            _commentRepo = commentRepo;
            _userRepo = userRepo;
            _commentCache = commentCache;   
            _userCache = userCache; 
        }
        public async Task<IEnumerable<CommentListResp>> CommentList(CommentListReq request)
        {
            long? cursor = null;
            if (!request.IsFirstPage() && long.TryParse(request.Cursor, out var parsedCursor))
            {
                cursor = parsedCursor;
            }

            var commentList = await _commentRepo.GetPaginatedComments(request.PostId,request.RootId, cursor,request.PageSize);
            /**
             * extract parentids to get user id
             */
            var parentIds = commentList.Select(c => c.ParentId).Where(c => c != 0).Distinct().ToList();
            /**
             * get parent comment detail from cache
             */
            var parentCommentMap = await _commentCache.GetBatch(parentIds);
            /**
             * extract user id
             */
            var userIds = commentList.Select(c => c.UserId).Distinct().ToList();
            var parentCommentUserIds = parentCommentMap.Values.Select(c => c.UserId).Distinct().ToList();
            /**
             * merge parent comment uid to current pagination comment uid
             */
            var allUserIds = userIds.Concat(parentCommentUserIds).Distinct().ToList();
            /*
             * get user info from cache
             */
            var userMap = await _userCache.GetBatch(allUserIds);

            /**
             * convert entity into response type 
             */
            return commentList.Select(comment =>
                {
                    var senderInfo = userMap.GetValueOrDefault(comment.UserId);
                    var parentComment = parentCommentMap.GetValueOrDefault(comment.ParentId,null);
                    var parentSenderInfo = parentComment != null ? userMap.GetValueOrDefault(parentComment.UserId) : null;
                    return new CommentListResp
                    {
                        CommentId = comment.Id,
                        Content = comment.CommentContent,
                        SenderUid = comment.UserId,
                        SenderName = senderInfo?.Username ?? null,
                        ReceiverUid = parentComment!=null?parentComment.UserId:0,
                        ReceiverName = parentSenderInfo?.Username ?? null,
                        ReplyTime = comment.CreatedTime,
                        PostId = comment.PostId,
                        RootId = comment.RootId,
                        ParentId = comment.ParentId
                    };
                }).ToList();
        }
    }
}
