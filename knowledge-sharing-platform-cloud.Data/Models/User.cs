using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knowledge_sharing_platform_cloud.Data.Models
{
    public class User
    {
        [Key] // Define Primary Key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Identity column
        [Column("id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required]
        [Column("email", TypeName = "NVARCHAR(320)")]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        [Column("password", TypeName = "NVARCHAR(255)")]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [Column("username", TypeName = "NVARCHAR(20)")]
        [MaxLength(20)]
        public string Username { get; set; }

        [Column("profile_url", TypeName = "NVARCHAR(255)")]
        [MaxLength(255)]
        public string? ProfileUrl { get; set; }

        [Column("stripe_account_id", TypeName = "NVARCHAR(255)")]
        [MaxLength(255)]
        public string? StripeAccountId { get; set; }

        [Column("description", TypeName = "TEXT")]
        public string? Description { get; set; }

        [Required]
        [Column("created_time", TypeName = "DATETIME2")]
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [Column("salt", TypeName = "NVARCHAR(255)")]
        [MaxLength(255)]
        public string Salt { get; set; }
    }
}
