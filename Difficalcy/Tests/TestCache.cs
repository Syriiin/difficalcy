using System.Collections.Generic;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class TestCacheDatabase : ICacheDatabase
    {
        private readonly Dictionary<string, string> dictionary = [];

        public Task<string> GetAsync(string key) => Task.FromResult(dictionary.GetValueOrDefault(key, null));

        public void Set(string key, string value) => dictionary.Add(key, value);
    }

    public class TestCache : ICache
    {
        private TestCacheDatabase _database = new TestCacheDatabase();

        public ICacheDatabase GetDatabase() => _database;
    }
}
