using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class TestBeatmapProvider(string resourceAssemblyName) : IBeatmapProvider
    {
        public Task EnsureBeatmap(string beatmapId)
        {
            var resourceName = $"{resourceAssemblyName}.Resources.{beatmapId}";
            var info = ResourceAssembly.GetManifestResourceInfo(resourceName);
            return Task.FromResult(info != null);
        }

        public Stream GetBeatmapStream(string beatmapId)
        {
            var resourceNamespace = "Testing.Beatmaps";
            var resourceName = $"{resourceNamespace}.{beatmapId}.osu";
            var fullResourceName = $"{resourceAssemblyName}.Resources.{resourceName}";
            var stream = ResourceAssembly.GetManifestResourceStream(fullResourceName);
            if (stream == null)
                throw new Exception($@"Unable to find resource ""{fullResourceName}"" in assembly ""{resourceAssemblyName}""");
            return stream;
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
