using System.Collections.Generic;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class InMemoryCacheDatabase : ICacheDatabase
    {
        private readonly Dictionary<string, string> dictionary = [];

        public Task<string> GetAsync(string key) => Task.FromResult(dictionary.GetValueOrDefault(key, null));

        public void Set(string key, string value) => dictionary.Add(key, value);
    }

    public class InMemoryCache : ICache
    {
        private InMemoryCacheDatabase _database = new InMemoryCacheDatabase();

        public ICacheDatabase GetDatabase() => _database;
    }
}
