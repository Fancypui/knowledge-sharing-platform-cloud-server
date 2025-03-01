using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CommentReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CommentResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface ICommentSerivce
    {

        public Task<IEnumerable<CommentListResp>> CommentList(CommentListReq request);
    }
}
