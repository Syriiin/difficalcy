using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.Services
{
    public class WebBeatmapProvider(IConfiguration configuration) : IBeatmapProvider
    {
        private readonly string _beatmapDirectory = configuration["BEATMAP_DIRECTORY"];
        private readonly HttpClient _httpClient = new();

        public async Task<bool> EnsureBeatmap(string beatmapId)
        {
            var beatmapPath = Path.Combine(_beatmapDirectory, $"{beatmapId}.osu");
            if (!File.Exists(beatmapPath))
            {
                using var response = await _httpClient.GetAsync($"https://osu.ppy.sh/osu/{beatmapId}");
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }

                using var fs = new FileStream(beatmapPath, FileMode.CreateNew);
                await response.Content.CopyToAsync(fs);
            }
            return true;
        }

        public Stream GetBeatmapStream(string beatmapId)
        {
            var beatmapPath = Path.Combine(_beatmapDirectory, beatmapId);
            return File.OpenRead(beatmapPath);
        }
    }
}
