namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.UserResp
{
    public class UserManagedChannelListResp
    {
        public long ChannelId { get; set; }

        public string Topic { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string ChannelBackgroundUrl { get; set; }
    }
}
