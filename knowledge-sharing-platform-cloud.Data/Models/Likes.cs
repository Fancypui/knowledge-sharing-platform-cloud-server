using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    public class Likes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required]
        [Column("post_id", TypeName = "BIGINT")]
        public long PostId { get; set; }

        [Required]
        [Column("user_id", TypeName = "BIGINT")]
        public long UserId { get; set; }

        [Required]
        [Column("like_status")]
        public bool LikeStatus { get; set; }
    }
}
