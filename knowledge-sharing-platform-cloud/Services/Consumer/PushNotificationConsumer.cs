using AWS.Messaging;
using knowledge_sharing_platform_cloud.Enum;
using knowledge_sharing_platform_cloud.Models.DTO;

namespace knowledge_sharing_platform_cloud.Services.Consumer
{
    public class PushNotificationConsumer : IMessageHandler<PushNotificationDTO>
    {
        private readonly IWebsocketService _websocketService;
        private readonly ILogger<PushNotificationConsumer> _logger;

        // Constructor to inject IWebsocketService
        public PushNotificationConsumer(IWebsocketService websocketService, ILogger<PushNotificationConsumer> logger)
        {
            _websocketService = websocketService;
            _logger = logger;
        }
        public Task<MessageProcessStatus> HandleAsync(MessageEnvelope<PushNotificationDTO> messageEnvelope, CancellationToken token = default)
        {
            /**
             * message envelope validation
             */
            if (messageEnvelope == null)
            {
                _logger.LogInformation("Message Envolope Null");
                return Task.FromResult(MessageProcessStatus.Failed());
            }
            if (messageEnvelope.Message == null)
            {
                _logger.LogInformation("Message Envolope's Message Null");
                return Task.FromResult(MessageProcessStatus.Failed());
            }
            PushNotificationDTO pushNotificationDTO = messageEnvelope.Message;
            PushNotificationType pushTypeEnum = pushNotificationDTO.Type;
            switch (pushTypeEnum)
            {
                case PushNotificationType.SEND_TO_Group:
                    /**
                     * push the message to client through webscokets
                     */
                    _websocketService.SendToGroup(pushNotificationDTO.UserIdList, pushNotificationDTO.GetResp());
                    break;
                case PushNotificationType.SEND_TO_INDIVIDUAL:
                    _websocketService.SendToGroup(pushNotificationDTO.UserIdList, pushNotificationDTO.GetResp());
                    break;
            }
            /**
             * message process success, remove message from SQS
             */
            return Task.FromResult(MessageProcessStatus.Success());

        }
    }
}
