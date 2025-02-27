using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
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
        public async Task<IActionResult> CreateChannel(CreateChannelReq createChannelReq)
        {
            try
            {
                CreateChannelResp createChannelResp = await _channelService.CreateChannel(createChannelReq);

                return CreatedAtAction(nameof(CreateChannel), createChannelResp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException?.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException?.Message);
            }
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinChannel(JoinChannelReq joinChannelReq)
        {
            try
            {
                ApiResult<JoinChannelResp> joinChannelResp = await _channelService.JoinChannel(joinChannelReq);

                return CreatedAtAction(nameof(JoinChannel), joinChannelResp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException?.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException?.Message);
            }
        }
    }
}
