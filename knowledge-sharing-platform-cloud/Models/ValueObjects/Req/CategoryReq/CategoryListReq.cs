using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq
{
    public class CategoryListReq
    {
        [Required(ErrorMessage = "Category id is required")]
        public long ChannelId { get; set; }
    }
}
