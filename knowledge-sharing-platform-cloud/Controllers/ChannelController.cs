using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("/channel")]
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
                var newChannel = await _channelService.CreateChannel(createChannelReq);

                return CreatedAtAction(nameof(CreateChannel), newChannel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.InnerException?.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException?.Message);
            }
        }
    }
}
