using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Difficalcy.Models;
using Microsoft.AspNetCore.Http;

namespace Difficalcy.Catch.Models
{
    public record CatchScore : Score, IValidatableObject
    {
        [Range(0, int.MaxValue)]
        public int? Combo { get; init; }

        /// <summary>
        /// The number of fruit and large droplet misses.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int Misses { get; init; } = 0; // fruit + large droplet misses

        [Range(0, int.MaxValue)]
        public int? SmallDroplets { get; init; }

        [Range(0, int.MaxValue)]
        public int? LargeDroplets { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Misses > 0 && Combo is null)
            {
                yield return new ValidationResult(
                    "Combo must be specified if Misses are greater than 0.",
                    [nameof(Combo)]
                );
            }
        }

        public static ValueTask<CatchScore> BindAsync(HttpContext context)
        {
            var (beatmapId, mods) = BindCommon(context);
            if (string.IsNullOrEmpty(beatmapId))
                return ValueTask.FromResult<CatchScore>(null);

            var query = context.Request.Query;

            return ValueTask.FromResult(
                new CatchScore
                {
                    BeatmapId = beatmapId,
                    Mods = mods,
                    Combo = ParseInt((string)query["Combo"]),
                    Misses = ParseInt((string)query["Misses"]) ?? 0,
                    SmallDroplets = ParseInt((string)query["SmallDroplets"]),
                    LargeDroplets = ParseInt((string)query["LargeDroplets"]),
                }
            );
        }
    }
}
