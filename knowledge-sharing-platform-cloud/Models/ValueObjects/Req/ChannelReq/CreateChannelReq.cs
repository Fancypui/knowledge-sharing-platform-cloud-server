namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.ChannelReq
{
    public class CreateChannelReq
    {
        public string Topic { get; set; }

        public string Description { get; set; }

        public string? ChannelImgUrl { get; set; }

        public string? ChannelImgBackground { get; set; }

        public long UserId { get; set; }

        public decimal SubscriptionFee { get; set; }

    }
}
