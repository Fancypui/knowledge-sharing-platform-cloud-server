using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq
{
    public class CreateCategoryReq
    {
        [Required]
        public long ChannelId { get; set; }
        [Required]
        public string CategoryName { get; set; }

    }
}
