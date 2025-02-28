namespace knowledge_sharing_platform_cloud.Models.DTO
{
    public class ChannelSummaryDTO
    {
        public long ChannelOwnerId { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }

        public string? ChannelImgUrl { get; set; }

        public string? ChannelImgBackground { get; set; }

        public decimal SubscriptionFee { get; set; }
    }
}
