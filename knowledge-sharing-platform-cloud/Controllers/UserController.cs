using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        //[HttpPost]
        //public async Task<ApiResult<CreateUserResp>> CreateUser(CreateUserReq createUserReq)
        //{
        //    CreateUserResp createUserResp = await _userService.CreateUser(createUserReq);

        //    return ApiResult<CreateUserResp>.ServiceSucess(createUserResp);
        //}

        [HttpGet("joinedChannels")]
        [Authorize]
        public async Task<ApiResult<IEnumerable<UserJoinedChannelListResp>>> UserJoinedChannelList([FromQuery] UserJoinedChannelListReq userJoinedChannelListReq)
        {
            IEnumerable<UserJoinedChannelListResp> userJoinedChannelListResp = await _userService.UserJoinedChannelList(long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<IEnumerable<UserJoinedChannelListResp>>.ServiceSucess(userJoinedChannelListResp);
        }
        [HttpPut("webPushSubscription")]
        [Authorize]
        public async Task<ApiResult<SaveWebPushResp>> updateWebPushSub([FromBody] SaveUserWebPushSubscription request)
        {
            await _userService.SaveUserWebPushSubcription(request,long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<SaveWebPushResp>.ServiceSucess(new SaveWebPushResp { Saved=true});
        }

        [HttpGet("managedChannels")]
        [Authorize]
        public async Task<ApiResult<IEnumerable<UserManagedChannelListResp>>> UserManagedChannelList([FromQuery] UserManagedChannelListReq userManagedChannelListReq)
        {
            IEnumerable<UserManagedChannelListResp> userManagedChannelListResp = await _userService.UserManagedChannelList(long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<IEnumerable<UserManagedChannelListResp>>.ServiceSucess(userManagedChannelListResp);
        }

        [HttpPost("register")]
        public async Task<ApiResult<UserRegisterResp>> UserRegistration([FromBody] UserRegisterReq request)
        {
            var resp = await _userService.userRegistration(request);

            return ApiResult<UserRegisterResp>.ServiceSucess(resp);
        }
        [HttpPost("login")]
        public async Task<ApiResult<UserLogInResp>> UserLogin([FromBody] UserLogInReq request)
        {
            var resp = await _userService.userLogIn(request);

            return ApiResult<UserLogInResp>.ServiceSucess(resp);
        }
        [HttpGet("userInfo")]
        [Authorize]
        public async Task<ApiResult<UserInfoResp>> GetUserInfo()
        {
            var resp = await _userService.GetUserInfo(long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<UserInfoResp>.ServiceSucess(resp);
        }
    }
}
