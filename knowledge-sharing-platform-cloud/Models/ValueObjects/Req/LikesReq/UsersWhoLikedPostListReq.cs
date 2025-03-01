using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.LikesReq
{
    public class UsersWhoLikedPostListReq : CursorBaseReq
    {
        [Required(ErrorMessage = "Post Id is required")]
        public long PostId { get; set; }
    }
}
