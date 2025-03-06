using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models.ChannelMember
{
    [Table("Channel_Member")]
    public class ChannelMember
    {
        [Key] // Marks it as a primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("channel_id")]
        public long ChannelId { get; set; }

        [Column("subscription_fee_paid")]
        public decimal SubscriptionFeePaid { get; set; }
    }
}
