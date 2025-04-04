using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required]
        [Column("category_name", TypeName = "NVARCHAR(50)")]
        [MaxLength(50)]
        public string CategoryName { get; set; }

        [Column("member_privilege")]
        public bool MemberPrivilege { get; set; }

        [Required]
        [Column("channel_id", TypeName = "BIGINT")]
        public long ChannelId { get; set; }
    }
}
