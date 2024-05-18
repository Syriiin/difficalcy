using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.Services
{
    public class WebBeatmapProvider(IConfiguration configuration) : IBeatmapProvider
    {
        private readonly string _beatmapDirectory = configuration["BEATMAP_DIRECTORY"];
        private readonly string _downloadMissingBeatmaps = configuration["DOWNLOAD_MISSING_BEATMAPS"];
        private readonly HttpClient _httpClient = new();

        public async Task EnsureBeatmap(string beatmapId)
        {
            var beatmapPath = GetBeatmapPath(beatmapId);
            if (!File.Exists(beatmapPath))
            {
                if (_downloadMissingBeatmaps != "true")
                    throw new BeatmapNotFoundException(beatmapId);

                using var response = await _httpClient.GetAsync($"https://osu.ppy.sh/osu/{beatmapId}");
                if (!response.IsSuccessStatusCode || response.Content.Headers.ContentLength == 0)
                    throw new BeatmapNotFoundException(beatmapId);

                using var fs = new FileStream(beatmapPath, FileMode.CreateNew);
                await response.Content.CopyToAsync(fs);
                if (fs.Length == 0)
                {
                    fs.Close();
                    File.Delete(beatmapPath);
                    throw new BeatmapNotFoundException(beatmapId);
                }
            }
        }

        public Stream GetBeatmapStream(string beatmapId)
        {
            var beatmapPath = GetBeatmapPath(beatmapId);
            return File.OpenRead(beatmapPath);
        }

        private string GetBeatmapPath(string beatmapId)
        {
            return Path.Combine(_beatmapDirectory, $"{beatmapId}.osu");
        }
    }
}
