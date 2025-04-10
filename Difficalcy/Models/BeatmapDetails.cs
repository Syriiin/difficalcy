namespace Difficalcy.Models
{
    public abstract record BeatmapDetails
    {
        // Metadata
        public string Artist { get; init; }
        public string Title { get; init; }
        public string DifficultyName { get; init; }
        public string Author { get; init; }

        // Statistics
        public int MaxCombo { get; init; }
        public double Length { get; init; }
        public int MinBPM { get; init; }
        public int MaxBPM { get; init; }
        public int CommonBPM { get; init; }
        public double BaseVelocity { get; init; }
        public double TickRate { get; init; }
    }
}
