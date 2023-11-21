using System.ComponentModel.DataAnnotations;

namespace Difficalcy.Models
{
    public abstract record Score
    {
        [Required]
        public string BeatmapId { get; init; }
        public int? Mods { get; init; }
    }
}
