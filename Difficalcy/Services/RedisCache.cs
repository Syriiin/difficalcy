using System.Threading.Tasks;
using StackExchange.Redis;

namespace Difficalcy.Services
{
    public class RedisCacheDatabase(IDatabase redisDatabase) : ICacheDatabase
    {
        public async Task<string> GetAsync(string key)
        {
            var redisValue = await redisDatabase.StringGetAsync(key);

            if (redisValue.IsNull)
                return null;

            return redisValue;
        }

        public void Set(string key, string value)
        {
            redisDatabase.StringSet(key, value, flags: CommandFlags.FireAndForget);
        }
    }

    public class RedisCache(IConnectionMultiplexer redis) : ICache
    {
        private readonly IConnectionMultiplexer _redis = redis;

        public ICacheDatabase GetDatabase()
        {
            return new RedisCacheDatabase(_redis.GetDatabase());
        }
    }
}
