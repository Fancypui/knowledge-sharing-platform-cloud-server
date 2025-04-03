namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp
{
    public class UserJoinedChannelListResp
    {
        public long ChannelId { get; set; }

        public string ChannelTopic { get; set; }

        public string ChannelOwnerName { get; set; }

        public string? ChannelImgUrl { get; set; }

        public string? ChannelBackgroundUrl { get; set; }

        public string? ChannelOwnerProfileUrl { get; set; }

    }
}
