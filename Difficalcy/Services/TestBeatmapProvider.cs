using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Difficalcy.Services
{
    public class TestBeatmapProvider : IBeatmapProvider
    {
        private string _resourceAssemblyName;

        public TestBeatmapProvider(string resourceAssemblyName)
        {
            _resourceAssemblyName = resourceAssemblyName;
        }

        public Task<bool> EnsureBeatmap(string beatmapId)
        {
            var resourceName = $"{_resourceAssemblyName}.Resources.{beatmapId}";
            var info = ResourceAssembly.GetManifestResourceInfo(resourceName);
            return Task.FromResult(info != null);
        }

        public Stream GetBeatmapStream(string beatmapId)
        {
            var resourceNamespace = "Testing.Beatmaps";
            var resourceName = $"{resourceNamespace}.{beatmapId}.osu";
            var fullResourceName = $"{_resourceAssemblyName}.Resources.{resourceName}";
            var stream = ResourceAssembly.GetManifestResourceStream(fullResourceName);
            if (stream == null)
                throw new Exception($@"Unable to find resource ""{fullResourceName}"" in assembly ""{_resourceAssemblyName}""");
            return stream;
        }

        private Assembly ResourceAssembly
        {
            get
            {
                string localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
                return Assembly.LoadFrom(Path.Combine(localPath, $"{_resourceAssemblyName}.dll"));
            }
        }
    }
}
