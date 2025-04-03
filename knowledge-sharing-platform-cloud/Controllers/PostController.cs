using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.PostResp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<ApiResult<CreatePostResp>> CreatePost([FromBody] CreatePostReq createPostReq)
        {
            CreatePostResp createPostResp = await _postService.CreatePost(createPostReq, long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<CreatePostResp>.ServiceSucess(createPostResp);
        }
        [HttpGet("page")]
        [Authorize]
        public async Task<ApiResult<CursorBasedResp<PostPageResp>>> GetPostPage([FromQuery] PostPageReq request)
        {
            
            var listData = await _postService.PostPage(request, long.Parse(HttpContext.Items["UserId"]?.ToString()));
            return ApiResult<CursorBasedResp<PostPageResp>>.ServiceSucess(listData);
        }
    }
}
