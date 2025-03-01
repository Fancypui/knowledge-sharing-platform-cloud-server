using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp;
using knowledge_sharing_platform_cloud.Services;
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

        [HttpPost]
        public async Task<ApiResult<CreateUserResp>> CreateUser(CreateUserReq createUserReq)
        {
            CreateUserResp createUserResp = await _userService.CreateUser(createUserReq);

            return ApiResult<CreateUserResp>.ServiceSucess(createUserResp);
        }

        [HttpGet("joinedChannels")]
        public async Task<ApiResult<IEnumerable<UserJoinedChannelListResp>>> UserJoinedChannelList([FromQuery] UserJoinedChannelListReq userJoinedChannelListReq)
        {
            IEnumerable<UserJoinedChannelListResp> userJoinedChannelListResp = await _userService.UserJoinedChannelList(userJoinedChannelListReq);

            return ApiResult<IEnumerable<UserJoinedChannelListResp>>.ServiceSucess(userJoinedChannelListResp);
        }

        [HttpGet("managedChannels")]
        public async Task<ApiResult<IEnumerable<UserManagedChannelListResp>>> UserManagedChannelList([FromQuery] UserManagedChannelListReq userManagedChannelListReq)
        {
            IEnumerable<UserManagedChannelListResp> userManagedChannelListResp = await _userService.UserManagedChannelList(userManagedChannelListReq);

            return ApiResult<IEnumerable<UserManagedChannelListResp>>.ServiceSucess(userManagedChannelListResp);
        }
    }
}
