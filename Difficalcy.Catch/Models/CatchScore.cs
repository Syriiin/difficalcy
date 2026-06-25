using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Difficalcy.Models;

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
    }
}
