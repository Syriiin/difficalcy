using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Services;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Mods;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using LazerMod = osu.Game.Rulesets.Mods.Mod;

namespace Difficalcy.Osu.Services
{
    public class OsuCalculatorService(ICache cache, IBeatmapProvider beatmapProvider)
        : CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>(cache)
    {
        private OsuRuleset OsuRuleset { get; } = new OsuRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(OsuRuleset)).GetName().Name;
                var packageVersion = Assembly
                    .GetAssembly(typeof(OsuRuleset))
                    .GetName()
                    .Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = OsuRuleset.Description,
                    CalculatorName = "Official osu!",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl =
                        $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}",
                };
            }
        }

        protected override async Task EnsureBeatmap(string beatmapId)
        {
            await beatmapProvider.EnsureBeatmap(beatmapId);
        }

        protected override (object, string) CalculateDifficultyAttributes(
            string beatmapId,
            Mod[] mods
        )
        {
            var workingBeatmap = GetWorkingBeatmap(beatmapId);
            var lazerMods = mods.Select(ModToLazerMod).ToArray();

            var difficultyCalculator = OsuRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes =
                difficultyCalculator.Calculate(lazerMods) as OsuDifficultyAttributes;

            // Serialising anonymous object with same names because some properties can't be serialised, and the built-in JsonProperty fields aren't on all required fields
            return (
                difficultyAttributes,
                JsonSerializer.Serialize(
                    new
                    {
                        difficultyAttributes.StarRating,
                        difficultyAttributes.MaxCombo,
                        difficultyAttributes.AimDifficulty,
                        difficultyAttributes.SpeedDifficulty,
                        difficultyAttributes.SpeedNoteCount,
                        difficultyAttributes.FlashlightDifficulty,
                        difficultyAttributes.SliderFactor,
                        difficultyAttributes.AimDifficultStrainCount,
                        difficultyAttributes.SpeedDifficultStrainCount,
                        difficultyAttributes.ApproachRate,
                        difficultyAttributes.OverallDifficulty,
                        difficultyAttributes.DrainRate,
                        difficultyAttributes.HitCircleCount,
                        difficultyAttributes.SliderCount,
                        difficultyAttributes.SpinnerCount,
                    }
                )
            );
        }

        protected override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(difficultyAttributesJson);
        }

        protected override OsuCalculation CalculatePerformance(
            OsuScore score,
            object difficultyAttributes
        )
        {
            var osuDifficultyAttributes = (OsuDifficultyAttributes)difficultyAttributes;

            var workingBeatmap = GetWorkingBeatmap(score.BeatmapId);
            var mods = score.Mods.Select(ModToLazerMod).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(OsuRuleset.RulesetInfo, mods);

            var combo =
                score.Combo
                ?? beatmap.HitObjects.Count
                    + beatmap.HitObjects.OfType<Slider>().Sum(s => s.NestedHitObjects.Count - 1);

            Dictionary<HitResult, int> statistics;
            double accuracy;

            if (mods.OfType<OsuModClassic>().Any(m => m.NoSliderHeadAccuracy.Value))
            {
                statistics = GetClassicHitResults(
                    beatmap.HitObjects.Count,
                    score.Misses,
                    score.Mehs,
                    score.Oks
                );
                accuracy = CalculateClassicAccuracy(statistics);
            }
            else
            {
                var maxSliderTails = beatmap.HitObjects.OfType<Slider>().Count();

                var maxSliderTicks = beatmap
                    .HitObjects.OfType<Slider>()
                    .Sum(s => s.NestedHitObjects.Count(x => x is SliderTick or SliderRepeat));

                statistics = GetLazerHitResults(
                    beatmap.HitObjects.Count,
                    maxSliderTails,
                    maxSliderTicks,
                    score.Misses,
                    score.Mehs,
                    score.Oks,
                    score.SliderTails,
                    score.SliderTicks
                );

                accuracy = CalculateLazerAccuracy(statistics, maxSliderTails, maxSliderTicks);
            }

            var scoreInfo = new ScoreInfo(beatmap.BeatmapInfo, OsuRuleset.RulesetInfo)
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods,
            };

            var performanceCalculator = OsuRuleset.CreatePerformanceCalculator();
            var performanceAttributes =
                performanceCalculator.Calculate(scoreInfo, osuDifficultyAttributes)
                as OsuPerformanceAttributes;

            return new OsuCalculation()
            {
                Difficulty = GetDifficultyFromDifficultyAttributes(osuDifficultyAttributes),
                Performance = GetPerformanceFromPerformanceAttributes(performanceAttributes),
                Accuracy = accuracy,
                Combo = combo,
            };
        }

        private CalculatorWorkingBeatmap GetWorkingBeatmap(string beatmapId)
        {
            using var beatmapStream = beatmapProvider.GetBeatmapStream(beatmapId);
            return new CalculatorWorkingBeatmap(OsuRuleset, beatmapStream);
        }

        private LazerMod ModToLazerMod(Mod mod)
        {
            var apiMod = new APIMod { Acronym = mod.Acronym };
            foreach (var setting in mod.Settings)
                apiMod.Settings.Add(setting.Key, setting.Value);

            return apiMod.ToMod(OsuRuleset);
        }

        private static Dictionary<HitResult, int> GetClassicHitResults(
            int hitResultCount,
            int countMiss,
            int countMeh,
            int countOk
        )
        {
            var countGreat = hitResultCount - countOk - countMeh - countMiss;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk },
                { HitResult.Meh, countMeh },
                { HitResult.Miss, countMiss },
            };
        }

        private static Dictionary<HitResult, int> GetLazerHitResults(
            int hitResultCount,
            int sliderTailCount,
            int sliderTickCount,
            int countMiss,
            int countMeh,
            int countOk,
            int? countSliderTails,
            int? countSliderTicks
        )
        {
            var countGreat = hitResultCount - countOk - countMeh - countMiss;

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, countOk },
                { HitResult.Meh, countMeh },
                { HitResult.Miss, countMiss },
                { HitResult.SliderTailHit, countSliderTails ?? sliderTailCount },
                { HitResult.LargeTickHit, countSliderTicks ?? sliderTickCount },
                {
                    HitResult.LargeTickMiss,
                    sliderTickCount - (countSliderTicks ?? sliderTickCount)
                },
            };
        }

        private static double CalculateClassicAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var countObjects = countGreat + countOk + countMeh + countMiss;

            if (countObjects == 0)
                return 1;

            var max = countObjects * 6;
            var total = (countGreat * 6) + (countOk * 2) + countMeh;

            return (double)total / max;
        }

        private static double CalculateLazerAccuracy(
            Dictionary<HitResult, int> statistics,
            int sliderTailCount,
            int sliderTickCount
        )
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMeh = statistics[HitResult.Meh];
            var countMiss = statistics[HitResult.Miss];
            var countSliderTails = statistics[HitResult.SliderTailHit];
            var countSliderTicks = statistics[HitResult.LargeTickHit];

            var countObject = countGreat + countOk + countMeh + countMiss;

            if (countObject == 0)
                return 1;

            var max = (countObject * 6) + (sliderTailCount * 3) + (sliderTickCount * 0.6);
            var total =
                (countGreat * 6)
                + (countOk * 2)
                + countMeh
                + (countSliderTails * 3)
                + (countSliderTicks * 0.6);

            return (double)total / max;
        }

        private static OsuDifficulty GetDifficultyFromDifficultyAttributes(
            OsuDifficultyAttributes difficultyAttributes
        )
        {
            return new OsuDifficulty()
            {
                Total = difficultyAttributes.StarRating,
                Aim = difficultyAttributes.AimDifficulty,
                Speed = difficultyAttributes.SpeedDifficulty,
                Flashlight = difficultyAttributes.FlashlightDifficulty,
            };
        }

        private static OsuPerformance GetPerformanceFromPerformanceAttributes(
            OsuPerformanceAttributes performanceAttributes
        )
        {
            return new OsuPerformance()
            {
                Total = performanceAttributes.Total,
                Aim = performanceAttributes.Aim,
                Speed = performanceAttributes.Speed,
                Accuracy = performanceAttributes.Accuracy,
                Flashlight = performanceAttributes.Flashlight,
            };
        }
    }
}
