using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    [Table("Comment")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }
        [Required]
        [Column("user_id", TypeName = "BIGINT")]
        public long UserId { get; set; }
        [Required]
        [Column("post_id", TypeName = "BIGINT")]
        public long PostId { get; set; }
        [Required]
        [Column("parent_id", TypeName = "BIGINT")]
        public long ParentId { get; set; }

        [Required]
        [Column("root_id", TypeName = "BIGINT")]
        public long RootId { get; set; }
        [Column("comment_content", TypeName = "TEXT")]
        public string? CommentContent { get; set; }
        [Required]
        [Column("created_time", TypeName = "DATETIME2")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
