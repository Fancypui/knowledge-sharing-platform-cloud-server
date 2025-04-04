using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    [Table("Channel_Member")]
    public class ChannelMember
    {
        [Key] // Marks it as a primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required] // Ensure user_id cannot be null
        [Column("user_id", TypeName = "BIGINT")]
        public long UserId { get; set; }

        [Required] // Ensure channel_id cannot be null
        [Column("channel_id", TypeName = "BIGINT")]
        public long ChannelId { get; set; }

        [Column("subscription_fee_paid", TypeName = "DECIMAL(10,2)")]
        public decimal? SubscriptionFeePaid { get; set; }
    }
}
