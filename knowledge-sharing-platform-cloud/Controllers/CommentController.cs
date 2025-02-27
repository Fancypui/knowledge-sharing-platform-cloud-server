using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly ICommentSerivce _commentSerivce;
        public CommentController(ICommentSerivce commentSerivce, ILogger<CommentController> logger)
        {
            _commentSerivce = commentSerivce;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ApiResult<IEnumerable<CommentListResp>>> CommentList([FromQuery] CommentListReq request)
        {
                var resp = await _commentSerivce.CommentList(request);
                return ApiResult<IEnumerable<CommentListResp>>.ServiceSucess(resp);    
        }
    }
}
