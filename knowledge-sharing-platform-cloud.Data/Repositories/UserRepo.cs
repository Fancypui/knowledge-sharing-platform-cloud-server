using knowledge_sharing_platform_cloud.Data.Models;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class UserRepo
    {
        private readonly UserContext _userContext;

        public UserRepo(UserContext userContext)
        {
            _userContext = userContext;
        }

        // dao
        public async Task<User> CreateUserAsync(User user)
        {
            _userContext.User.Add(user);
            await _userContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            User user = await _userContext.User.FindAsync(id);

            return user;
        }

        public async Task<User> UpdateUserAsync(User updatedUser)
        {
            _userContext.User.Update(updatedUser);
            await _userContext.SaveChangesAsync();

            return updatedUser;
        }
    }
}
