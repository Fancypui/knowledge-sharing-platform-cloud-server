namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp
{
    public class UserJoinedChannelListResp
    {
        public long ChannelId { get; set; }

        public string ChannelTopic { get; set; }

        public string ChannelOwnerName { get; set; }

        public string? ChannelImgBackground { get; set; }

        public string? ChannelOwnerProfileUrl { get; set; }

    }
}
