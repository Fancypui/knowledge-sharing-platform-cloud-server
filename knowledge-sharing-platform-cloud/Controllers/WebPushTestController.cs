using AWS.Messaging;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("WebPush")]
    [ApiController]
    public class WebPushTestController : Controller
    {
        private IMessagePublisher _messagePublisher;
        private UserRepo _userRepo;
        public WebPushTestController(
            UserRepo userRepo,
            IMessagePublisher messagePublisher
        )
        {
            _userRepo = userRepo;
            _messagePublisher = messagePublisher;
        }
        

        [HttpGet]
        public async Task Index()
        {
            var user = await _userRepo.GetUserByIdAsync(11000);
            if (user == null || user.WebPushSubscription == null)
            {
                return;
            }
            var webPushPaymentMsgDTO = new WebPushPaymentMsgDTO
            {
                Subscription = user.WebPushSubscription,
                Text = "Success 123",
                ChannelId = 11001,
                Title = "Channel Payment Status",
                RedirectUrl = "http://localhost:3000/channel/11000"
            };
            await _messagePublisher.PublishAsync(webPushPaymentMsgDTO);
        }
    }
}
