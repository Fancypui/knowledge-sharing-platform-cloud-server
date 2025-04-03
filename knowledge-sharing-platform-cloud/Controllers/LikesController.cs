using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<ApiResult<LikeDislikePostResp>> LikeDislikePost(LikeDislikePostReq likeDislikePostReq)
        {
            LikeDislikePostResp likeDislikePostResp = await _likesService.LikeDislikePost(likeDislikePostReq, long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<LikeDislikePostResp>.ServiceSucess(likeDislikePostResp);
        }

        [HttpGet("post")]
        [Authorize]
        public async Task<ApiResult<CursorBasedResp<UsersWhoLikedPostListResp>>> UserWhoLikedPostList([FromQuery] UsersWhoLikedPostListReq usersWhoLikedPostListReq)
        {
            CursorBasedResp<UsersWhoLikedPostListResp> usersWhoLikedPostListResp = await _likesService.UsersWhoLikedPostList(usersWhoLikedPostListReq);

            return ApiResult<CursorBasedResp<UsersWhoLikedPostListResp>>.ServiceSucess(usersWhoLikedPostListResp);
        }
    }
}
