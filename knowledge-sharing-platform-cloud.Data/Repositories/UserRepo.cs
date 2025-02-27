using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly UserContext _userContext;
        public UserRepo(UserContext userContext)
        {
            _userContext = userContext;
        }

        // dao
        public async Task<User> CreateUserAsync(User user)
        {
            var newUser = _userContext.User.Add(user);
            await _userContext.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> userListByIds(List<long> userIds)
        {
            return await _userContext.User
                    .Where(c => userIds.Contains(c.Id))
                    .ToListAsync();
        }
    }
}
