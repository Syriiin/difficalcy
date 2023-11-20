namespace Difficalcy.Models
{
    public abstract record Score
    {
        public string BeatmapId { get; init; }
        public int? Mods { get; init; }
    }
}
