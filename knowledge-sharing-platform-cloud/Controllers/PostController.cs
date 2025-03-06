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
        public async Task<ApiResult<CreatePostResp>> CreatePost([FromBody] CreatePostReq createPostReq)
        {
            CreatePostResp createPostResp = await _postService.CreatePost(createPostReq,11000);

            return ApiResult<CreatePostResp>.ServiceSucess(createPostResp);
        }
        [HttpGet("page")]
        public async Task<ApiResult<CursorBasedResp<PostPageResp>>> GetPostPage([FromQuery] PostPageReq request)
        {
            
            var listData = await _postService.PostPage(request, 11000);
            return ApiResult<CursorBasedResp<PostPageResp>>.ServiceSucess(listData);
        }
    }
}
