using knowledge_sharing_platform_cloud.Models.DTO;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq
{
    public class CreatePostReq
    {

        public long CategoryId { get; set; }

        public string PostTitle { get; set; }

        public string PostBody { get; set; }

        public List<PostImageUrl>? PostImageUrls { get; set; }

        public class PostImageUrl
        {
            public string ImageUrl { get; set; }
        }
    }
}
