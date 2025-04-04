using System.Formats.Asn1;
using knowledge_sharing_platform_cloud.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class UserRepo
    {
        private readonly ApplicationContext _applicationContext;

        public UserRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        // dao
        public async Task<User> CreateUserAsync(User user)
        {
            _applicationContext.User.Add(user);
            await _applicationContext.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> UserListByIds(List<long> userIds)
        {
            return await _applicationContext.User
                    .Where(c => userIds.Contains(c.Id)).ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            User user = await _applicationContext.User.FindAsync(id);

            return user;
        }


        public async Task<User> UpdateUserAsync(User updatedUser)
        {
            _applicationContext.User.Update(updatedUser);
            await _applicationContext.SaveChangesAsync();

            return updatedUser;
        }

        public async Task<User> SaveNewUser(User newUser)
        {
            _applicationContext.User.Add(newUser);
            await _applicationContext.SaveChangesAsync();
            return newUser;
        }

        public async Task<User?> getByEmail(string email)
        {
            var user = await _applicationContext.User
                .FirstOrDefaultAsync(u => u.Email == email);
 
            return user;
        }
    }
}
