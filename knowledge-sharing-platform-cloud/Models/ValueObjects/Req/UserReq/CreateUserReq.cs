using System.ComponentModel.DataAnnotations.Schema;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.UserReq
{
    public class CreateUserReq
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }

        public string? ProfileUrl { get; set; }

        public string Description { get; set; }
    }
}
