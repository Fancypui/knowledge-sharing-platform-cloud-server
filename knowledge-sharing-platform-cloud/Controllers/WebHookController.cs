using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("/webhook")]
    [ApiController]
    public class WebhookController : Controller
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly IConfiguration _configuration;

        public WebhookController(ILogger<WebhookController> logger,IConfiguration configuration)
        {
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

                Console.WriteLine(stripeEvent.Type); 

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    Console.WriteLine("✅ Payment succeeded!");
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