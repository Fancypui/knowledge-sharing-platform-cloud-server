using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models.Comment
{
   
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }
        [Column("user_id")]
        public long UserId { get; set; }
        [Column("post_id")]
        public long PostId { get; set; }
        [Column("parent_id")]
        public long ParentId { get; set; }

        [Column("root_id")]
        public long RootId { get; set; }
        [Column("comment_content")]
        public string? CommentContent { get; set; }
        [Column("created_time")]
        public DateTime CreatedTime { get; set; }
    }
}
