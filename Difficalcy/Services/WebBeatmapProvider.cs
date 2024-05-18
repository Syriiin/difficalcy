using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Difficalcy.Services
{
    public class WebBeatmapProvider(IConfiguration configuration) : IBeatmapProvider
    {
        private readonly string _beatmapDirectory = configuration["BEATMAP_DIRECTORY"];
        private readonly HttpClient _httpClient = new();

        public async Task EnsureBeatmap(string beatmapId)
        {
            var beatmapPath = GetBeatmapPath(beatmapId);
            if (!File.Exists(beatmapPath))
            {
                using var response = await _httpClient.GetAsync($"https://osu.ppy.sh/osu/{beatmapId}");
                if (!response.IsSuccessStatusCode)
                    throw new BadHttpRequestException("Beatmap not found");

                using var fs = new FileStream(beatmapPath, FileMode.CreateNew);
                if (fs.Length == 0)
                    throw new BadHttpRequestException("Beatmap not found");

                await response.Content.CopyToAsync(fs);
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
