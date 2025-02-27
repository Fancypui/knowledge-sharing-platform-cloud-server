namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req
{
    public class CreateChannelReq
    {
        public string topic { get; set; }

        public string description { get; set; }
        
        public string? channelImgUrl { get; set; }

        public string? channelImgBackground { get; set; }

        public long userId { get; set; }

        public decimal subscriptionFee { get; set; }

    }
}
