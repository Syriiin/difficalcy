using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Difficalcy.Services
{
    public class WebBeatmapProvider(
        IConfiguration configuration,
        ILogger<WebBeatmapProvider> logger
    ) : IBeatmapProvider
    {
        private readonly string _beatmapDirectory = configuration["BEATMAP_DIRECTORY"];
        private readonly string _downloadMissingBeatmaps = configuration[
            "DOWNLOAD_MISSING_BEATMAPS"
        ];
        private readonly string _beatmapDownloadUrl =
            configuration["BEATMAP_DOWNLOAD_URL"] ?? "https://osu.ppy.sh/osu/{0}";
        private readonly HttpClient _httpClient = new();

        public async Task EnsureBeatmap(string beatmapId)
        {
            var beatmapPath = GetBeatmapPath(beatmapId);
            if (!File.Exists(beatmapPath))
            {
                if (_downloadMissingBeatmaps != "true")
                {
                    logger.LogWarning(
                        "Beatmap {BeatmapId} not found and downloading is disabled",
                        beatmapId
                    );
                    throw new BeatmapNotFoundException(beatmapId);
                }

                logger.LogInformation("Downloading beatmap {BeatmapId}", beatmapId);

                using var response = await _httpClient.GetAsync(
                    string.Format(_beatmapDownloadUrl, beatmapId)
                );
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(
                        "Failed to download beatmap {BeatmapId}, status code {StatusCode}",
                        beatmapId,
                        response.StatusCode
                    );
                    throw new BeatmapNotFoundException(beatmapId);
                }

                if (response.Content.Headers.ContentLength == 0)
                {
                    logger.LogWarning(
                        "Downloaded beatmap {BeatmapId} response was empty",
                        beatmapId
                    );
                    throw new BeatmapNotFoundException(beatmapId);
                }

                using var fs = new FileStream(beatmapPath, FileMode.CreateNew);
                await response.Content.CopyToAsync(fs);
                if (fs.Length == 0)
                {
                    logger.LogWarning(
                        "Downloaded beatmap {BeatmapId} was empty, deleting",
                        beatmapId
                    );
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
