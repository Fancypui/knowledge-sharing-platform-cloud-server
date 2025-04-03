using System.Formats.Asn1;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<User>> UserListByIds(List<long> userIds)
        {
            return await _userContext.User
                    .Where(c => userIds.Contains(c.Id)).ToListAsync();
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

        public async Task<User> SaveNewUser(User newUser)
        {
            _userContext.User.Add(newUser);
            await _userContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<User?> getByEmail(string email)
        {
            var user = await _userContext.User
                .FirstOrDefaultAsync(u => u.Email == email);
 
            return user;
        }
    }
}
