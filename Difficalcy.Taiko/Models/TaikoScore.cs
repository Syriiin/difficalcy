using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Difficalcy.Models;
using Microsoft.AspNetCore.Http;

namespace Difficalcy.Taiko.Models
{
    public record TaikoScore : Score, IValidatableObject
    {
        [Range(0, int.MaxValue)]
        public int? Combo { get; init; }

        [Range(0, int.MaxValue)]
        public int Misses { get; init; } = 0;

        [Range(0, int.MaxValue)]
        public int Oks { get; init; } = 0;

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

        public static ValueTask<TaikoScore> BindAsync(HttpContext context)
        {
            var (beatmapId, mods) = BindCommon(context);
            if (string.IsNullOrEmpty(beatmapId))
                return ValueTask.FromResult<TaikoScore>(null);

            var query = context.Request.Query;

            return ValueTask.FromResult(
                new TaikoScore
                {
                    BeatmapId = beatmapId,
                    Mods = mods,
                    Combo = ParseInt((string)query["Combo"]),
                    Misses = ParseInt((string)query["Misses"]) ?? 0,
                    Oks = ParseInt((string)query["Oks"]) ?? 0,
                }
            );
        }
    }
}
