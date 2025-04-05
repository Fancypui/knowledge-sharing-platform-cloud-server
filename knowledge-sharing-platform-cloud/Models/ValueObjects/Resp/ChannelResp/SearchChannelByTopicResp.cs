namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp
{
    public class SearchChannelByTopicResp
    {
        public long ChannelId { get; set; }

        public string ChannelTopic { get; set; }

        public string ChannelDesc { get; set; }

        public decimal? SubscriptionFee { get; set; }

        public string ChannelImgUrl { get; set; }

        public string ChannelImgBackground { get; set; }

        //public bool IsUserJoined { get; set; }
    }
}
