using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CommentReq
{
    public class CommentListReq : CursorBaseReq
    {
        [Required(ErrorMessage = "Post Id is required")]
        public long PostId { get; set; }
        /**
         * root id is the level of hierachy in the comment table
         */
        [Required(ErrorMessage = "Root Id is required.")]
        public long RootId { get; set; }

    }
}
