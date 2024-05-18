using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class TestBeatmapProvider(string resourceAssemblyName) : IBeatmapProvider
    {
        public Task EnsureBeatmap(string beatmapId)
        {
            var resourceName = GetResourceName(beatmapId);
            _ = ResourceAssembly.GetManifestResourceInfo(resourceName) ?? throw new BeatmapNotFoundException(beatmapId);
            return Task.CompletedTask;
        }

        public Stream GetBeatmapStream(string beatmapId)
        {
            var resourceName = GetResourceName(beatmapId);
            return ResourceAssembly.GetManifestResourceStream(resourceName);
        }

        private string GetResourceName(string beatmapId)
        {
            var resourceNamespace = "Testing.Beatmaps";
            var resourceName = $"{resourceNamespace}.{beatmapId}.osu";
            return $"{resourceAssemblyName}.Resources.{resourceName}";
        }

        private Assembly ResourceAssembly
        {
            get
            {
                string localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                return Assembly.LoadFrom(Path.Combine(localPath, $"{resourceAssemblyName}.dll"));
            }
        }
    }
}
