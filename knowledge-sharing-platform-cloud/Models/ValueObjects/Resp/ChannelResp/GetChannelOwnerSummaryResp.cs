namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.ChannelResp
{
    public class GetChannelOwnerSummaryResp
    {
        public long ChannelId { get; set; }

        public long UserId { get; set; }

        public string Username { get; set; }

        public string UserDescription { get; set; }

        public string? UserProfileUrl { get; set; }
    }
}
