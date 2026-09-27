using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public interface IBeatmapProvider
    {
        public Task EnsureBeatmap(string beatmapId);

        public string GetBeatmapPath(string beatmapId);
    }
}
