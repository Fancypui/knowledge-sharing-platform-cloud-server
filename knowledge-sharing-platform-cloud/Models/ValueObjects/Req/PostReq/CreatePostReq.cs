using knowledge_sharing_platform_cloud.Models.DTO;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq
{
    public class CreatePostReq
    {
        public long UserId { get; set; }

        public long CategoryId { get; set; }

        public string PostTitle { get; set; }

        public string PostBody { get; set; }

        public List<PostImageUrlDTO> PostImageUrl { get; set; }
    }
}
