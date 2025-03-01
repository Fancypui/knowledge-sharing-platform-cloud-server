using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.ChannelReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("channel")]
    [ApiController]
    public class ChannelController : Controller
    {
        private readonly IChannelService _channelService;
        private readonly ILogger<ChannelController> _logger;

        public ChannelController(IChannelService channelService, ILogger<ChannelController> logger)
        {
            _channelService = channelService;
            _logger = logger;
        }


        [HttpPost]
        public async Task<ApiResult<CreateChannelResp>> CreateChannel(CreateChannelReq createChannelReq)
        {
            CreateChannelResp createChannelResp = await _channelService.CreateChannel(createChannelReq);

            return ApiResult<CreateChannelResp>.ServiceSucess(createChannelResp);
        }

        [HttpPost("join")]
        public async Task<ApiResult<JoinChannelResp>> JoinChannel(JoinChannelReq joinChannelReq)
        {
            JoinChannelResp joinChannelResp = await _channelService.JoinChannel(joinChannelReq);

            return ApiResult<JoinChannelResp>.ServiceSucess(joinChannelResp);
        }

        [HttpGet("summary")]
        public async Task<ApiResult<GetChannelSummaryResp>> GetChannelSummary([FromQuery]GetChannelSummaryReq getChannelSummaryReq)
        {
            GetChannelSummaryResp getChannelSummaryResp = await _channelService.GetChannelSummary(getChannelSummaryReq);

            return ApiResult<GetChannelSummaryResp>.ServiceSucess(getChannelSummaryResp);
        }

        [HttpGet("ownerSummary")]
        public async Task<ApiResult<GetChannelOwnerSummaryResp>> GetChannelOwnerSummary([FromQuery]GetChannelOwnerSummaryReq getChannelOwnerSummaryReq)
        {
            GetChannelOwnerSummaryResp getChannelOwnerSummaryResp = await _channelService.GetChannelOwnerSummary(getChannelOwnerSummaryReq);

            return ApiResult<GetChannelOwnerSummaryResp>.ServiceSucess(getChannelOwnerSummaryResp);
        }

        [HttpGet("search")]
        public async Task<ApiResult<IEnumerable<SearchChannelByTopicResp>>> SearchChannelByTopic([FromQuery] SearchChannelByTopicReq searchChannelByTopicReq)
        {
            IEnumerable<SearchChannelByTopicResp> channelList = await _channelService.SearchChannelByTopic(searchChannelByTopicReq);

            return ApiResult<IEnumerable<SearchChannelByTopicResp>>.ServiceSucess(channelList);
        }
        [HttpGet("leaderboard/page")]
        public async Task<ApiResult<CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>>>> ChannelLeaderboardPage([FromQuery] CursorBaseReq request)
        {
            CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>> channelList = await _channelService.ChannelLeaderboardList(request);

            return ApiResult<CursorBasedResp<IEnumerable<ChannelLeaderboardListResp>>>.ServiceSucess(channelList);
        }
    }
}
