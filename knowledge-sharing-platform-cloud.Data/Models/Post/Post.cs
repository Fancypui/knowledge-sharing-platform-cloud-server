using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Models.Post
{
    public class Post
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("body")]
        public string Body { get; set; }

        [Column("post_img_url")]
        public string? PostImgUrl { get; set; }

        [Column("user_id")]
        public long UserId { get; set; }

        [Column("category_id")]
        public long CategoryId { get; set; }

        [Column("deleted_status")]
        public bool DeletedStatus { get; set; }

        [Column("created_time")]
        public DateTime CreatedTime { get; set; }

        [Column("updated_time")]
        public DateTime UpdatedTime { get; set; }
    }
}
