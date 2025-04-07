namespace knowledge_sharing_platform_cloud.Models.DTO
{
    public class StripeWebhookEventDTO
    {
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public decimal AmountPaid { get; set; }
        public string CheckoutSessionId { get; set; }
        public string CheckoutSessionPaymentStatus { get; set; }
    }
}
