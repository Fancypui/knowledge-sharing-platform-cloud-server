using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledge_sharing_platform_cloud.Data.Models.Channel
{
    public class Channel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("topic")]
        public string Topic { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("channel_img_url")]
        public string? ChannelImgUrl { get; set; }

        [Column("channel_img_background")]
        public string? ChannelImgBackground { get; set; }

        [Column("user_id")]
        public long UserId {  get; set; }

        [Column("last_post_id")]
        public long? LastPostId {  get; set; }

        [Column("subscription_fee")]
        public decimal SubscriptionFee {  get; set; }

        [Column("active_time")]
        public DateTime? ActiveTime {  get; set; }

        [Column("stripe_price_id")]
        public string StripePriceId {  get; set; }

        [Column("total_member")]
        public int TotalMember {  get; set; }

        [Column("total_post")]
        public int TotalPost {  get; set; }

        [Column("created_time")]
        public DateTime CreatedTime { get; set; }
    }
}
