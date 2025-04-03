namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.LikesResp
{
    public class UsersWhoLikedPostListResp
    {
        public long? UserId { get; set; }

        public string? Username { get; set; }

        public string? ProfileUrl { get; set; }

        public long LikeId { get; set; }
    }
}
