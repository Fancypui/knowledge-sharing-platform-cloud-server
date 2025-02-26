using knowledge_sharing_platform_cloud.Data.Models;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public interface IUserRepo
    {
        Task<User> CreateUserAsync(User user);
    }
}