using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Mania.Models;
using Difficalcy.Models;
using Difficalcy.Services;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Mania.Difficulty;
using osu.Game.Rulesets.Mania.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using LazerMod = osu.Game.Rulesets.Mods.Mod;

namespace Difficalcy.Mania.Services
{
    public class ManiaCalculatorService(ICache cache, IBeatmapProvider beatmapProvider)
        : CalculatorService<
            ManiaScore,
            ManiaDifficulty,
            ManiaPerformance,
            ManiaCalculation,
            ManiaBeatmapDetails
        >(cache)
    {
        private readonly IBeatmapProvider _beatmapProvider = beatmapProvider;
        private ManiaRuleset ManiaRuleset { get; } = new ManiaRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(ManiaRuleset)).GetName().Name;
                var packageVersion = Assembly
                    .GetAssembly(typeof(ManiaRuleset))
                    .GetName()
                    .Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = ManiaRuleset.Description,
                    CalculatorName = "Official osu!mania",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl =
                        $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}",
                };
            }
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await _beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(
            string beatmapId,
            Mod[] mods
        )
        {
            var workingBeatmap = GetWorkingBeatmap(beatmapId);
            var lazerMods = mods.Select(ModToLazerMod).ToArray();

            var difficultyCalculator = ManiaRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes =
                difficultyCalculator.Calculate(lazerMods) as ManiaDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (
                difficultyAttributes,
                JsonSerializer.Serialize(
                    new { difficultyAttributes.StarRating, difficultyAttributes.MaxCombo }
                )
            );
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<ManiaDifficultyAttributes>(difficultyAttributesJson);
        }

        protected override ManiaCalculation CalculatePerformance(
            ManiaScore score,
            object difficultyAttributes
        )
        {
            var maniaDifficultyAttributes = (ManiaDifficultyAttributes)difficultyAttributes;

            var scoreInfo = GetScoreInfo(score);

            var performanceCalculator = ManiaRuleset.CreatePerformanceCalculator();
            var performanceAttributes =
                performanceCalculator.Calculate(scoreInfo, maniaDifficultyAttributes)
                as ManiaPerformanceAttributes;

            return new ManiaCalculation()
            {
                Difficulty = GetDifficultyFromDifficultyAttributes(maniaDifficultyAttributes),
                Performance = GetPerformanceFromPerformanceAttributes(performanceAttributes),
                Accuracy = scoreInfo.Accuracy,
            };
        }

        protected override ManiaBeatmapDetails GetBeatmapDetailsSync(string beatmapId)
        {
            var workingBeatmap = GetWorkingBeatmap(beatmapId);
            var beatmap = workingBeatmap.GetPlayableBeatmap(ManiaRuleset.RulesetInfo);

            var scoreInfo = new ScoreInfo(beatmap.BeatmapInfo, ManiaRuleset.RulesetInfo) { };

            var noteCount = beatmap.HitObjects.OfType<Note>().Count();

            return new ManiaBeatmapDetails()
            {
                Artist = beatmap.Metadata.Artist,
                Title = beatmap.Metadata.Title,
                Author = beatmap.Metadata.Author.Username,
                DifficultyName = beatmap.BeatmapInfo.DifficultyName,
                MaxCombo = beatmap.GetMaxCombo(),
                Length = beatmap.CalculatePlayableLength(),
                MinBPM = (int)Math.Round(beatmap.ControlPointInfo.BPMMinimum),
                MaxBPM = (int)Math.Round(beatmap.ControlPointInfo.BPMMaximum),
                CommonBPM = (int)Math.Round(60000 / beatmap.GetMostCommonBeatLength()),
                NoteCount = noteCount,
                HoldNoteCount = beatmap.HitObjects.OfType<HoldNote>().Count(),
                KeyCount = beatmap.Difficulty.CircleSize,
                Accuracy = Math.Round(beatmap.Difficulty.OverallDifficulty, 2),
                DrainRate = Math.Round(beatmap.Difficulty.DrainRate, 2),
                BaseVelocity = Math.Round(beatmap.Difficulty.SliderMultiplier, 2),
                TickRate = Math.Round(beatmap.Difficulty.SliderTickRate, 2),
            };
        }

        private ScoreInfo GetScoreInfo(ManiaScore score)
        {
            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = score.Mods.Select(ModToLazerMod).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(ManiaRuleset.RulesetInfo, mods);

            var hitObjectCount = beatmap.HitObjects.Count;
            var holdNoteTailCount = beatmap.HitObjects.OfType<HoldNote>().Count();
            var statistics = GetHitResults(
                hitObjectCount + holdNoteTailCount,
                score.Misses,
                score.Mehs,
                score.Oks,
                score.Goods,
                score.Greats
            );
            var accuracy = CalculateAccuracy(statistics);

            return new ScoreInfo(beatmap.BeatmapInfo, ManiaRuleset.RulesetInfo)
            {
                Accuracy = accuracy,
                MaxCombo = beatmap.GetMaxCombo(),
                Statistics = statistics,
                Mods = mods,
            };
        }

        private CalculatorWorkingBeatmap GetWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = _beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(ManiaRuleset, beatmapStream);
        }

        private LazerMod ModToLazerMod(Mod mod)
        {
            var apiMod = new APIMod { Acronym = mod.Acronym };
            foreach (var setting in mod.Settings)
                apiMod.Settings.Add(setting.Key, setting.Value);

            return apiMod.ToMod(ManiaRuleset);
        }

        private static Dictionary<HitResult, int> GetHitResults(
            int hitResultCount,
            int countMiss,
            int countMeh,
            int countOk,
            int countGood,
            int countGreat
        )
        {
            var countPerfect =
                hitResultCount - (countMiss + countMeh + countOk + countGood + countGreat);

            return new Dictionary<HitResult, int>
            {
                [HitResult.Perfect] = countPerfect,
                [HitResult.Great] = countGreat,
                [HitResult.Good] = countGood,
                [HitResult.Ok] = countOk,
                [HitResult.Meh] = countMeh,
                [HitResult.Miss] = countMiss,
            };
        }

        private static double CalculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countPerfect = statistics[HitResult.Perfect];
            var countGreat = statistics[HitResult.Great];
            var countGood = statistics[HitResult.Good];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var total = countPerfect + countGreat + countGood + countOk + countMeh + countMiss;

            if (total == 0)
                return 1;

            return (double)(
                    (6 * countPerfect)
                    + (6 * countGreat)
                    + (4 * countGood)
                    + (2 * countOk)
                    + countMeh
                ) / (6 * total);
        }

        private static ManiaDifficulty GetDifficultyFromDifficultyAttributes(
            ManiaDifficultyAttributes difficultyAttributes
        )
        {
            return new ManiaDifficulty() { Total = difficultyAttributes.StarRating };
        }

        private static ManiaPerformance GetPerformanceFromPerformanceAttributes(
            ManiaPerformanceAttributes performanceAttributes
        )
        {
            return new ManiaPerformance()
            {
                Total = performanceAttributes.Total,
                Difficulty = performanceAttributes.Difficulty,
            };
        }
    }
}
