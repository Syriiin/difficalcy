using System.IO;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.IO;
using osu.Game.Rulesets;
using osu.Game.Rulesets.Osu;
using osu.Game.Skinning;

namespace Difficalcy.Mania.Services
{
    public class CalculatorWorkingBeatmap : WorkingBeatmap
    {
        private readonly Beatmap _beatmap;

        public CalculatorWorkingBeatmap(Ruleset ruleset, string beatmapPath, int beatmapId) : this(ruleset, readFromFile(beatmapPath), beatmapId) { }

        private CalculatorWorkingBeatmap(Ruleset ruleset, Beatmap beatmap, int beatmapId) : base(beatmap.BeatmapInfo, null)
        {
            _beatmap = beatmap;

            // Only valid maps will be either osu! converts or osu!mania maps
            _beatmap.BeatmapInfo.Ruleset = beatmap.BeatmapInfo.RulesetID == 0 ? (new OsuRuleset()).RulesetInfo : ruleset.RulesetInfo;
            _beatmap.BeatmapInfo.OnlineBeatmapID = beatmapId;
        }

        private static Beatmap readFromFile(string filename)
        {
            using (var stream = File.OpenRead(filename))
            using (var reader = new LineBufferedReader(stream))
                return Decoder.GetDecoder<Beatmap>(reader).Decode(reader);
        }

        protected override IBeatmap GetBeatmap() => _beatmap;
        protected override Texture GetBackground() => null;
        protected override Track GetBeatmapTrack() => null;
        protected override ISkin GetSkin() => null;
        public override Stream GetStream(string storagePath) => null;
    }
}