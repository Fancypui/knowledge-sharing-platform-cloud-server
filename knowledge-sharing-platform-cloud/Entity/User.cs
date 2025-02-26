using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Entity
{
    public class User
    {
        public long Id { get; set; }

        public string Email { get; set; }


        public string Password { get; set; }

        public string Username { get; set; }

        public string? Profile_Url { get; set; }
    }
}
