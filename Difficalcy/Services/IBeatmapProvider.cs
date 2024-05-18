using System.IO;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public interface IBeatmapProvider
    {
        public Task EnsureBeatmap(string beatmapId);

        public Stream GetBeatmapStream(string beatmapId);
    }
}
