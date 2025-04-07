namespace knowledge_sharing_platform_cloud.Models.DTO
{
    public class WebPushPaymentMsgDTO
    {
        public long ChannelId { get; set; }
        public string Subscription {  get; set; }

        public string Title { get; set; }

        public string Text { get; set; }    
        public string RedirectUrl { get; set; }
    }
}
