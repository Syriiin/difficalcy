using System.Threading.Tasks;
using StackExchange.Redis;

namespace Difficalcy.Services
{
    public class RedisCacheDatabase : ICacheDatabase
    {
        private IDatabase _redisDatabase;

        public RedisCacheDatabase(IDatabase redisDatabase)
        {
            _redisDatabase = redisDatabase;
        }

        public async Task<string> GetAsync(string key)
        {
            var redisValue = await _redisDatabase.StringGetAsync(key);

            if (redisValue.IsNull)
                return null;

            return redisValue;
        }

        public void Set(string key, string value)
        {
            _redisDatabase.StringSet(key, value, flags: CommandFlags.FireAndForget);
        }
    }

    public class RedisCache : ICache
    {
        private IConnectionMultiplexer _redis;

        public RedisCache(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public ICacheDatabase GetDatabase()
        {
            return new RedisCacheDatabase(_redis.GetDatabase());
        }
    }
}
