using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    [Table("Channel")]
    public class Channel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required]
        [Column("topic", TypeName = "VARCHAR(30)")]
        [MaxLength(30)]
        public string Topic { get; set; }

        [Column("description", TypeName = "TEXT")]
        public string? Description { get; set; }

        [Column("channel_img_url", TypeName = "VARCHAR(255)")]
        [MaxLength(255)]
        public string? ChannelImgUrl { get; set; }

        [Column("channel_img_background", TypeName = "VARCHAR(255)")]
        [MaxLength(255)]
        public string? ChannelImgBackground { get; set; }

        [Required]
        [Column("user_id",TypeName="BIGINT")]
        public long UserId { get; set; }

        [Column("last_post_id", TypeName ="BIGINT")]
        public long? LastPostId { get; set; }

        [Column("subscription_fee", TypeName = "DECIMAL(10,2)")]
        public decimal? SubscriptionFee { get; set; }

        [Column("active_time", TypeName = "DATETIME")]
        public DateTime? ActiveTime { get; set; }

        [Column("stripe_price_id", TypeName = "VARCHAR(255)")]
        [MaxLength(255)]
        public string? StripePriceId { get; set; }

        [Column("total_member", TypeName = "INT")]
        public int TotalMember { get; set; }

        [Column("total_post", TypeName = "INT")]
        public int TotalPost { get; set; }

        [Required]
        [Column("created_time", TypeName = "DATETIME2")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    }
}
