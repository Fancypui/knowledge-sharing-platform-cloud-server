using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.PostResp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("post")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        public async Task<ApiResult<CreatePostResp>> CreatePost(CreatePostReq createPostReq)
        {
            CreatePostResp createPostResp = await _postService.CreatePost(createPostReq);

            return ApiResult<CreatePostResp>.ServiceSucess(createPostResp);
        }
    }
}
