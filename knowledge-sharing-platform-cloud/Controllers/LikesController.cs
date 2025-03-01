using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("likes")]
    [ApiController]
    public class LikesController : Controller
    {
        private readonly ILikesService _likesService;


        public LikesController(ILikesService likesService)
        {
            _likesService = likesService;
        }


        [HttpPost]
        public async Task<ApiResult<LikeDislikePostResp>> LikeDislikePost(LikeDislikePostReq likeDislikePostReq)
        {
            LikeDislikePostResp likeDislikePostResp = await _likesService.LikeDislikePost(likeDislikePostReq);

            return ApiResult<LikeDislikePostResp>.ServiceSucess(likeDislikePostResp);
        }  
    }
}
