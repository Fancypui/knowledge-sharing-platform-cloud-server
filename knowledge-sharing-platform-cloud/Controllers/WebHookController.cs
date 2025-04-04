using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly IChannelService _channelService;
        private readonly ILogger<WebhookController> _logger;
        private readonly IConfiguration _configuration;

        public WebhookController(IChannelService channelService, ILogger<WebhookController> logger, IConfiguration configuration)
        {
            _channelService = channelService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("stripe")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _configuration["StripeWebhookSecretKey"]);

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var checkoutSession = stripeEvent.Data.Object as Stripe.Checkout.Session;

                        if (checkoutSession.PaymentStatus == "paid")
                        {
                            var userId = checkoutSession.Metadata["userId"];
                            var channelId = checkoutSession.Metadata["channelId"];
                            decimal totalPaid = (decimal)checkoutSession.AmountTotal;

                            Console.WriteLine("it works");
                            _channelService.JoinChannelSuccess(userId, channelId, totalPaid);
                        }
                        else if (checkoutSession.PaymentStatus == "unpaid")
                        {
                            Console.WriteLine($"Payment for session {checkoutSession.Id} was not successful.");
                            //_channelService.JoinChannelFail();
                        }
                        break;

                    case "payment_intent.payment_failed":
                        var paymentIntent = stripeEvent.Data.Object as Stripe.PaymentIntent;
                        Console.WriteLine($"Payment failed for PaymentIntent {paymentIntent.Id}: {paymentIntent.LastPaymentError?.Message}");
                        break;

                    case "invoice.payment_failed":
                        var invoice = stripeEvent.Data.Object as Stripe.Invoice;
                        Console.WriteLine($"Invoice {invoice.Id} payment failed for customer {invoice.CustomerId}");
                        break;

                    default:
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest($"Webhook Error: {e.Message}");
            }
        }
    }
}