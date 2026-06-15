using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Difficalcy.Models;
using Microsoft.AspNetCore.Http;

namespace Difficalcy.Mania.Models
{
    public record ManiaScore : Score
    {
        [Range(0, int.MaxValue)]
        public int Misses { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Mehs { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Oks { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Goods { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Greats { get; init; } = 0;

        public static ValueTask<ManiaScore> BindAsync(HttpContext context)
        {
            var (beatmapId, mods) = BindCommon(context);
            if (string.IsNullOrEmpty(beatmapId))
                return ValueTask.FromResult<ManiaScore>(null);

            var query = context.Request.Query;

            return ValueTask.FromResult(
                new ManiaScore
                {
                    BeatmapId = beatmapId,
                    Mods = mods,
                    Misses = ParseInt((string)query["Misses"]) ?? 0,
                    Mehs = ParseInt((string)query["Mehs"]) ?? 0,
                    Oks = ParseInt((string)query["Oks"]) ?? 0,
                    Goods = ParseInt((string)query["Goods"]) ?? 0,
                    Greats = ParseInt((string)query["Greats"]) ?? 0,
                }
            );
        }
    }
}
