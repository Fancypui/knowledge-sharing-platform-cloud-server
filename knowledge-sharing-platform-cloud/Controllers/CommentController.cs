using AWS.Messaging;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Enum;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.DTO;
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
        private readonly IMessagePublisher _messagePublisher;
        public CommentController(ICommentSerivce commentSerivce, ILogger<CommentController> logger, IMessagePublisher messagePublisher)
        {
            _commentSerivce = commentSerivce;
            _logger = logger;
            _messagePublisher = messagePublisher;
        }
        [HttpGet]
        public async Task<ApiResult<IEnumerable<CommentListResp>>> CommentList([FromQuery] CommentListReq request)
        {
                var resp = await _commentSerivce.CommentList(request);
                return ApiResult<IEnumerable<CommentListResp>>.ServiceSucess(resp);    
        }

        [HttpGet("testmq")]
        public async Task<IActionResult> PublishOrder()
        {
            var data = new WSRespBase<string>()
            {
                Type = (int)WSRespTypeEnum.PAYMENT_SUCESS,
                Data = "Payment Success",
            };
            var message = new ChannelLeaderboardDTO()
            {
                UserIdList = new List<long> { 1 },
                Type = PushNotificationType.SEND_TO_INDIVIDUAL,
            };
            message.SetResp(data);

            // Publish the OrderInfo to SNS, using the generic publisher
            await _messagePublisher.PublishAsync(message);

            return Ok();
        }
    }
}
