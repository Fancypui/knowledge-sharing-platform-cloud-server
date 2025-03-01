using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req
{
    public class PostPageReq: CursorBaseReq
    {
        [Required(ErrorMessage = "Channel Category Id is required")]
        public long ChannelCateogryId {  get; set; }
       
    }
}
