using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Services;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace Difficalcy.Osu.Services
{
    public class OsuCalculatorService : CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
    {
        private readonly IBeatmapProvider _beatmapProvider;
        private OsuRuleset OsuRuleset { get; } = new OsuRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(OsuRuleset)).GetName().Name;
                var packageVersion = Assembly.GetAssembly(typeof(OsuRuleset)).GetName().Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = OsuRuleset.Description,
                    CalculatorName = "Official osu!",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl = $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}"
                };
            }
        }

        public OsuCalculatorService(ICache cache, IBeatmapProvider beatmapProvider) : base(cache)
        {
            _beatmapProvider = beatmapProvider;
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await _beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(OsuScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

            var difficultyCalculator = OsuRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as OsuDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                AimDifficulty = difficultyAttributes.AimDifficulty,
                SpeedDifficulty = difficultyAttributes.SpeedDifficulty,
                FlashlightDifficulty = difficultyAttributes.FlashlightDifficulty,
                ApproachRate = difficultyAttributes.ApproachRate,
                OverallDifficulty = difficultyAttributes.OverallDifficulty,
                DrainRate = difficultyAttributes.DrainRate,
                HitCircleCount = difficultyAttributes.HitCircleCount,
                SliderCount = difficultyAttributes.SliderCount,
                SpinnerCount = difficultyAttributes.SpinnerCount
            }));
        }

        protected override OsuDifficulty GetDifficultyFromDifficultyAttributes(object difficultyAttributes)
        {
            var osuDifficultyAttributes = (OsuDifficultyAttributes)difficultyAttributes;
            return new OsuDifficulty()
            {
                Total = osuDifficultyAttributes.StarRating,
                Aim = osuDifficultyAttributes.AimDifficulty,
                Speed = osuDifficultyAttributes.SpeedDifficulty,
                Flashlight = osuDifficultyAttributes.FlashlightDifficulty
            };
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        protected override OsuPerformance CalculatePerformance(OsuScore score, object difficultyAttributes)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(OsuRuleset.RulesetInfo, mods);

            var combo = score.Combo ?? beatmap.HitObjects.Count + beatmap.HitObjects.OfType<Slider>().Sum(s => s.NestedHitObjects.Count - 1);
            var statistics = determineHitResults(score.Accuracy ?? 1, beatmap.HitObjects.Count, score.Misses ?? 0, score.Mehs, score.Oks);
            var accuracy = score.Accuracy ?? calculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo()
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = OsuRuleset.CreatePerformanceCalculator();
            var performanceAttributes = performanceCalculator.Calculate(scoreInfo, (OsuDifficultyAttributes)difficultyAttributes) as OsuPerformanceAttributes;

            return new OsuPerformance()
            {
                Total = performanceAttributes.Total,
                Aim = performanceAttributes.Aim,
                Speed = performanceAttributes.Speed,
                Accuracy = performanceAttributes.Accuracy,
                Flashlight = performanceAttributes.Flashlight
            };
        }

        protected override OsuCalculation GetCalculation(OsuDifficulty difficulty, OsuPerformance performance)
        {
            return new OsuCalculation()
            {
                Difficulty = difficulty,
                Performance = performance
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = _beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(OsuRuleset, beatmapStream, beatmapId);
        }

        private Dictionary<HitResult, int> determineHitResults(double targetAccuracy, int hitResultCount, int countMiss, int? countMeh, int? countOk)
        {
            // Adapted from https://github.com/ppy/osu-tools/blob/cf5410b04f4e2d1ed2c50c7263f98c8fc5f928ab/PerformanceCalculator/Simulate/OsuSimulateCommand.cs#L57-L91
            int countGreat;

            if (countMeh != null || countOk != null)
            {
                countGreat = hitResultCount - (countOk ?? 0) - (countMeh ?? 0) - countMiss;
            }
            else
            {
                // Let Great=6, Ok=2, Meh=1, Miss=0. The total should be this.
                var targetTotal = (int)Math.Round(targetAccuracy * hitResultCount * 6);

                // Start by assuming every non miss is a meh
                // This is how much increase is needed by greats and oks
                var delta = targetTotal - (hitResultCount - countMiss);

                // Each great increases total by 5 (great-meh=5)
                countGreat = delta / 5;
                // Each ok increases total by 1 (ok-meh=1). Covers remaining difference.
                countOk = delta % 5;
                // Mehs are left over. Could be negative if impossible value of amountMiss chosen
                countMeh = hitResultCount - countGreat - countOk - countMiss;
            }

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk ?? 0 },
                { HitResult.Meh, countMeh ?? 0 },
                { HitResult.Miss, countMiss }
            };
        }

        private double calculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var total = countGreat + countOk + countMeh + countMiss;

            return (double)((6 * countGreat) + (2 * countOk) + countMeh) / (6 * total);
        }
    }
}
