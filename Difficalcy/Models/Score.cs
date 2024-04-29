using System.ComponentModel.DataAnnotations;

namespace Difficalcy.Models
{
    public abstract record Score
    {
        [Required]
        public string BeatmapId { get; init; }

        [Range(0, int.MaxValue)]
        public int Mods { get; init; } = 0;
    }
}
