using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.PostResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IPostService
    {
        Task<CreatePostResp> CreatePost(CreatePostReq createPostReq);
        Task<CursorBasedResp<IEnumerable<PostPageResp>>> PostPage(PostPageReq request, long uid);
    }
}