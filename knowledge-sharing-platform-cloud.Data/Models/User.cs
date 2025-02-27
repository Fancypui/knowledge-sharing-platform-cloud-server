using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public long Id { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("profile_url")]
        public string? ProfileUrl { get; set; }

        [Column("stripe_account_id")]
        public string? StripeAccountId { get; set; }
    }
}
