using Difficalcy.Models;

namespace Difficalcy.Mania.Models
{
    public record ManiaBeatmapDetails : BeatmapDetails
    {
        // Hit Objects
        public int NoteCount { get; init; }
        public int HoldNoteCount { get; init; }

        // Difficulty Settings
        public double KeyCount { get; init; }
        public double Accuracy { get; init; }
        public double DrainRate { get; init; }
    }
}
