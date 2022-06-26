using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class DummyCacheDatabase : ICacheDatabase
    {
        public async Task<string> GetAsync(string key) => await Task.Run(() => (string)null);

        public void Set(string key, string value) { }
    }

    public class DummyCache : ICache
    {
        private DummyCacheDatabase _database = new DummyCacheDatabase();

        public ICacheDatabase GetDatabase() => _database;
    }
}
