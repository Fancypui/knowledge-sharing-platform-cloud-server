using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface ICommentSerivce
    {

        public Task<IEnumerable<CommentListResp>> CommentList(CommentListReq request);
    }
}
