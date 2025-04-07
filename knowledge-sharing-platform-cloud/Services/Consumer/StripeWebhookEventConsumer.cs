using AWS.Messaging;
using knowledge_sharing_platform_cloud.Models.DTO;

namespace knowledge_sharing_platform_cloud.Services.Consumer
{
    public class StripeWebhookEventConsumer : IMessageHandler<StripeWebhookEventDTO>
    {
        private readonly ILogger<StripeWebhookEventConsumer> _logger;
        private readonly IChannelService _channelService;

        public StripeWebhookEventConsumer(ILogger<StripeWebhookEventConsumer> logger, IChannelService channelService)
        {
            _logger = logger;
            _channelService = channelService;
        }

        public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<StripeWebhookEventDTO> messageEnvelope, CancellationToken token = default)
        {
            if (messageEnvelope == null || messageEnvelope.Message == null)
            {
                _logger.LogInformation("Message Envelope or Message is null");
                return MessageProcessStatus.Failed();
            }

            var stripeWebhookEventDTO = messageEnvelope.Message;

            Console.WriteLine("enter consumer");

            if (stripeWebhookEventDTO.CheckoutSessionPaymentStatus == "paid")
            {
                try
                {
                    await _channelService.JoinChannelSuccess(stripeWebhookEventDTO);
                    Console.WriteLine("join channel success");
                    return MessageProcessStatus.Success();
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Join channel success error");
                    return MessageProcessStatus.Failed();
                }                
            }

            try
            {
                await _channelService.JoinChannelFail(stripeWebhookEventDTO);
                Console.WriteLine("join channel fail");
                return MessageProcessStatus.Success();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Join channel fail error");
                return MessageProcessStatus.Failed();
            }

        }
    }
}
