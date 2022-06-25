using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class DummyCacheDatabase : ICacheDatabase
    {
        public async Task<string> GetAsync(string key)
        {
            return await Task.Run(() => (string)null);
        }

        public void Set(string key, string value)
        {
        }
    }

    public class DummyCache : ICache
    {
        public ICacheDatabase GetDatabase()
        {
            return new DummyCacheDatabase();
        }
    }
}
