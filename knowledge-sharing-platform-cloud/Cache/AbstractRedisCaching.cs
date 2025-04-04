
using System.Text.Json;
using knowledge_sharing_platform_cloud.Constant;
using StackExchange.Redis;

namespace knowledge_sharing_platform_cloud.Cache
{
    public abstract class AbstractRedisCaching<KEY, RETURN>
    {
        private readonly Type _returnType;

        private readonly IDatabase _redisDB;

        /**
         * get return type
         */
        public AbstractRedisCaching(IConnectionMultiplexer redis)
        {
            var baseType = GetType().BaseType;
            if(baseType !=null && baseType.IsGenericType)
            {
                _returnType = baseType.GetGenericArguments()[1];
                _redisDB = redis.GetDatabase();
            }
        }
        public void Delete(KEY key)
        {
            DeleteBatch(new List<KEY> { key });
        }

        public void DeleteBatch(List<KEY> keys)
        {
            if (keys == null || !keys.Any())
            {
                return;
            }

            RedisKey[] redisKeys = keys.Select(key => (RedisKey)GetKey(key)).ToArray();

            _redisDB.KeyDelete(redisKeys);
        }

        public async Task<RETURN> Get(KEY key)
        {
            var result = await GetBatch(new List<KEY> { key });
            if (result.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }

        public async Task<Dictionary<KEY, RETURN>> GetBatch(List<KEY> keys)
        {
            if (keys == null || !keys.Any())
            {
                return new Dictionary<KEY, RETURN>();
            }
            RedisKey[] redisKeys = keys.Select(key => (RedisKey)GetKey(key)).ToArray();

            RedisValue[] values = _redisDB.StringGet(redisKeys);
            var result = new Dictionary<KEY, RETURN>();
            var needLoadingFromDB = new List<KEY>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (!values[i].IsNullOrEmpty)
                {
                    result[keys[i]] = JsonSerializer.Deserialize<RETURN>(values[i]);
                }
                else
                {
                    needLoadingFromDB.Add(keys[i]);
                }
            }
            var load = new Dictionary<KEY, RETURN>();
            if (needLoadingFromDB.Any())
            {
                load = await Load(needLoadingFromDB);
                if (load.Any())
                {
                    foreach (var kvp in load)
                    {
                        string serializedValue = JsonSerializer.Serialize(kvp.Value);
                        _redisDB.StringSet(GetKey(kvp.Key), serializedValue,
                            TimeSpan.FromMinutes(RedisConstant.KEY_EXPIRY_DURATION));
                        result[kvp.Key] = kvp.Value;
                    }
                   
                }
            }
            return result;


        }
        public abstract Task<Dictionary<KEY, RETURN>> Load(List<KEY> keys);

        public abstract string GetKey(KEY key);

        

     
    }
}
