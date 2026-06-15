using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Difficalcy.Models
{
    public abstract record Score
    {
        [Required]
        public string BeatmapId { get; init; }

        public Mod[] Mods { get; init; } = [];

        protected static (string BeatmapId, Mod[] Mods) BindCommon(
            Microsoft.AspNetCore.Http.HttpContext context
        )
        {
            var query = context.Request.Query;

            var beatmapId = (string)query["BeatmapId"];

            var mods = new List<Mod>();
            for (int i = 0; ; i++)
            {
                var acronym = (string)query[$"Mods[{i}].Acronym"];
                if (acronym == null)
                    break;

                var settings = new Dictionary<string, string>();
                for (int j = 0; ; j++)
                {
                    var key = (string)query[$"Mods[{i}].Settings[{j}].Key"];
                    var value = (string)query[$"Mods[{i}].Settings[{j}].Value"];
                    if (key == null || value == null)
                        break;
                    settings[key] = value;
                }

                mods.Add(new Mod { Acronym = acronym, Settings = settings });
            }

            return (beatmapId, [.. mods]);
        }

        protected static int? ParseInt(string value) =>
            string.IsNullOrEmpty(value) ? null
            : int.TryParse(value, out var result) ? result
            : null;
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

            var settingsString = string.Join(
                ",",
                Settings
                    .OrderBy(setting => setting.Key)
                    .Select(setting => $"{setting.Key}={setting.Value}")
            );

            return $"{Acronym}({settingsString})";
        }
    }
}
