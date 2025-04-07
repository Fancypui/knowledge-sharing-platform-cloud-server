using AWS.Messaging;
using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Models.DTO;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class WebPushServiceImpl : IWebPushService
    {
        private readonly UserRepo _userRepo;

        private readonly string _frontendDomainName;

        private readonly IMessagePublisher _messagePublisher;

        public WebPushServiceImpl(
            UserRepo userRepo,
            IMessagePublisher messagePublisher,
            IConfiguration config
        )
        {
            _userRepo = userRepo;
            _messagePublisher = messagePublisher;
            _frontendDomainName = config["FrontEndDomainName"];
        }
        public async void PushChannelPaymentMsgToClientWeb(long userId, long channelId, string pushMsg, 
            string redirectUrlWithoutFrontendDomainName)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if(user== null || user.WebPushSubscription == null)
            {
                return;
            }
            var webPushPaymentMsgDTO = new WebPushPaymentMsgDTO
            {
                Subscription = user.WebPushSubscription,
                Text = pushMsg,
                ChannelId = channelId,
                Title = "Channel Payment Status",
                RedirectUrl = _frontendDomainName + redirectUrlWithoutFrontendDomainName
            };
            await _messagePublisher.PublishAsync(webPushPaymentMsgDTO);


        }
    }
}
