using System.Reflection;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CommentReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CommentResp;
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
        private readonly PostRepo _postRepo;

        public CommentServiceImpl(CommentRepo commentRepo, PostRepo postRepo,
            UserRepo userRepo,CommentCache commentCache, UserCache userCache)
        {
            _commentRepo = commentRepo;
            _userRepo = userRepo;
            _commentCache = commentCache;   
            _userCache = userCache;
            _postRepo = postRepo;
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

        public async Task<ReplyPostCommentResp> ReplyPostComment(ReplyPostCommentReq request, long uid)
        {
            /**
             * validation
             */
            if(request.CommentContent.Length < 1)
            {
                throw new BusinessException("Comment Content is empty, reply fail");
            }
            var post = await _postRepo.GetPostById(request.PostId);
            if(post ==null || post.DeletedStatus)
            {
                throw new BusinessException("Post does not exist, reply fail");
            }
            if(request.ParentId==0 && request.RootId != 0)
            {
                throw new BusinessException("Parent Id cannot be 0 if root id is not 0");
            }
            Comment? parentCommentRecord = null;
            if (request.ParentId != 0)
            {
                parentCommentRecord = await _commentCache.Get(request.ParentId);
                if (parentCommentRecord == null || parentCommentRecord.RootId != request.RootId)
                {
                    throw new BusinessException("Parent Comment's root id does not align with request body root id/" +
                        "Parent Comment not exist");

                }   
            }
            var parentId = request.ParentId;
            long rootId = 0;
            if (request.RootId == 0 && request.ParentId != 0)
            {
                rootId = request.ParentId;
            }
            else if (request.RootId != 0 && request.ParentId!=0)
            {
                rootId = request.RootId;
            }
            var saveComment = new Comment()
            {
                ParentId = parentId,
                RootId = rootId,
                CommentContent = request.CommentContent,
                PostId = request.PostId,
                UserId = uid,

            };
            /**
             * save db
             */
            var comment = await _commentRepo.CreateCommentAsync(saveComment);
            return new ReplyPostCommentResp()
            {
                CommentId = comment.Id,
            };


        }
    }
}
