using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Difficalcy.Models
{
    public abstract record Score
    {
        [Required]
        public string BeatmapId { get; init; }

        public Mod[] Mods { get; init; } = [];
    }

    public record Mod
    {
        [Required]
        public string Acronym { get; init; }

        public Dictionary<string, string> Settings { get; init; } = [];

        public override string ToString()
        {
            if (Settings.Count == 0)
                return Acronym;

            var settingsString = string.Join(",", Settings.OrderBy(setting => setting.Key).Select(setting => $"{setting.Key}={setting.Value}"));

            return $"{Acronym}({settingsString})";
        }
    }
}
