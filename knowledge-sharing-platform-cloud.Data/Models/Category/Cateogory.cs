using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models.Category
{
    public class Category
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("category_name")]
        public string CategoryName { get; set; }

        [Column("member_privilege")]
        public bool MemberPrivilege { get; set; }

        [Column("channel_id")]
        public long ChannelId { get; set; }
    }
}
