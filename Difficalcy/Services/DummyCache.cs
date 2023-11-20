using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class DummyCacheDatabase : ICacheDatabase
    {
        public Task<string> GetAsync(string key) => Task.FromResult((string)null);

        public void Set(string key, string value) { }
    }

    public class DummyCache : ICache
    {
        private DummyCacheDatabase _database = new DummyCacheDatabase();

        public ICacheDatabase GetDatabase() => _database;
    }
}
