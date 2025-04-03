using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CommentReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CommentResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface ICommentSerivce
    {

        public Task<CursorBasedResp<CommentListResp>> CommentList(CommentListReq request);

        public Task<ReplyPostCommentResp> ReplyPostComment(ReplyPostCommentReq request, long uid);
 
    }
}
