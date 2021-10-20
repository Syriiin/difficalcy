namespace Difficalcy.Models
{
    public abstract record Score
    {
        public int BeatmapId { get; init; }
        public int? Mods { get; init; }
    }
}
