namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp
{
    public class GetChannelSummaryResp
    {
        public long ChannelId { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }

        public string? ChannelImgUrl { get; set; }

        public string? ChannelImgBackground { get; set; }

        public int TotalMember { get; set; }

        public int TotalPost { get; set; }

        public double OperationDuration { get; set; }

        public decimal? SubscriptionFee { get; set; }
    }
}
