using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public interface ICacheDatabase
    {
        Task<string> GetAsync(string key);

        void Set(string key, string value);
    }

    public interface ICache
    {
        ICacheDatabase GetDatabase();
    }
}
