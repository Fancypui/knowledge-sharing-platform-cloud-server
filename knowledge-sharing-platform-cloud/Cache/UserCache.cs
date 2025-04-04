using System.Xml.Linq;
using knowledge_sharing_platform_cloud.Constant;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Cache
{
    public class UserCache : AbstractRedisCaching<long, User>
    {
        private readonly UserRepo _userRepo;
        public UserCache(IConnectionMultiplexer redis,UserRepo userRepo) : base(redis)
        {
            _userRepo = userRepo;
        }

        public override string GetKey(long key)
        {
            return RedisConstant.GetKey(RedisConstant.USER_INFO,key);
        }

        public override async Task<Dictionary<long, User>> Load(List<long> keys)
        {
            var userList = await _userRepo.UserListByIds(keys);
            return userList.ToDictionary(c => c.Id, c => c);
        }
    }
}
