using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class TestBeatmapProvider(string resourceAssemblyName) : IBeatmapProvider
    {
        private readonly Dictionary<string, string> _cachedPaths = [];

        public Task EnsureBeatmap(string beatmapId)
        {
            var resourceName = GetResourceName(beatmapId);
            _ =
                ResourceAssembly.GetManifestResourceInfo(resourceName)
                ?? throw new BeatmapNotFoundException(beatmapId);
            return Task.CompletedTask;
        }

        public string GetBeatmapPath(string beatmapId)
        {
            if (_cachedPaths.TryGetValue(beatmapId, out var cachedPath))
                return cachedPath;

            var resourceName = GetResourceName(beatmapId);
            using var stream = ResourceAssembly.GetManifestResourceStream(resourceName);

            var tempPath = Path.GetTempFileName();
            using var fileStream = File.Create(tempPath);
            stream.CopyTo(fileStream);

            _cachedPaths[beatmapId] = tempPath;
            return tempPath;
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
