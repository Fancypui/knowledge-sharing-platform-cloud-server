using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req
{
    public class ReplyPostCommentReq
    {
        [Required(ErrorMessage = "Post Id is required")]
        public long PostId { get; set; }
        [Required(ErrorMessage = "Parent Id is required")]
        public long ParentId { get; set; }
        [Required(ErrorMessage = "Comment Content is required")]
        public string CommentContent { get; set; }
        [Required(ErrorMessage = "Root id is required")]
        public long RootId { get; set; }    


    }
}
