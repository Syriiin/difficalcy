using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Difficalcy.Models;

namespace Difficalcy.Taiko.Models
{
    public record TaikoScore : Score, IValidatableObject
    {
        [Range(0, 1)]
        public double? Accuracy { get; init; }

        [Range(0, int.MaxValue)]
        public int? Combo { get; init; }

        [Range(0, int.MaxValue)]
        public int? Misses { get; init; }

        [Range(0, int.MaxValue)]
        public int? Oks { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Misses is not null && Combo is null)
            {
                yield return new ValidationResult("Combo must be specified if Misses are specified.", [nameof(Combo)]);
            }
        }
    }
}
