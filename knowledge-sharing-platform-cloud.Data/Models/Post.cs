
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace knowledge_sharing_platform_cloud.Data.Models
{
    [Table("Post")]
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required]
        [Column("title", TypeName = "VARCHAR(50)")]
        [MaxLength(50)]
        public string Title { get; set; }

        [Column("body", TypeName = "TEXT")]
        public string Body { get; set; }

        [Column("post_img_url", TypeName = "NVARCHAR(MAX)")]
        public string? PostImgUrl { get; set; }

        [Required]
        [Column("user_id", TypeName = "BIGINT")]
        public long UserId { get; set; }

        [Required]
        [Column("category_id", TypeName = "BIGINT")]
        public long CategoryId { get; set; }

        [Column("deleted_status")]
        public bool DeletedStatus { get; set; }

        [Required]
        [Column("created_time", TypeName = "DATETIME2")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Required]
        [Column("updated_time", TypeName = "DATETIME2")]
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    }
}
