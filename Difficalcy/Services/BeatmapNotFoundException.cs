using System;

namespace Difficalcy.Services
{
    public class BeatmapNotFoundException(string beatmapId)
        : Exception($"Beatmap {beatmapId} not found") { }
}
