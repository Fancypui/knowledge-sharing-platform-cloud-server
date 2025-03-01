namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp
{
    public class PostPageResp
    {
        public long PostCreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorProfileUrl { get; set; }

        public DateTime PostCreatedTime { get; set; }

        public string PostContent { get; set; }

        public string[] ImgUrls;

        public Boolean PostByChannelOwner { get; set; }
        public Boolean LikeByCurrentUser { get; set; }
    }
}
