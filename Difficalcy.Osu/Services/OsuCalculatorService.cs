using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Osu.Models;
using Difficalcy.Services;
using Microsoft.Extensions.Configuration;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Osu.Difficulty;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using StackExchange.Redis;

namespace Difficalcy.Osu.Services
{
    public class OsuCalculatorService : CalculatorService<OsuScore, OsuDifficulty, OsuPerformance, OsuCalculation>
    {
        private readonly IConfiguration _configuration;
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

        public OsuCalculatorService(IConfiguration configuration, IConnectionMultiplexer redis) : base(redis)
        {
            _configuration = configuration;
        }

        public override async Task EnsureBeatmap(int beatmapId)
        {
            var beatmapPath = Path.Combine(_configuration["BEATMAP_DIRECTORY"], beatmapId.ToString());
            if (!File.Exists(beatmapPath))
            {
                var myWebClient = new WebClient();
                await myWebClient.DownloadFileTaskAsync($"https://osu.ppy.sh/osu/{beatmapId}", beatmapPath);
            }
        }

        public override (object, string) CalculateDifficulty(OsuScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = OsuRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

            var difficultyCalculator = OsuRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as OsuDifficultyAttributes;

            // Serialising anonymous object with same names because Mods and Skills can't be serialised
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                AimStrain = difficultyAttributes.AimStrain,
                SpeedStrain = difficultyAttributes.SpeedStrain,
                FlashlightRating = difficultyAttributes.FlashlightRating,
                ApproachRate = difficultyAttributes.ApproachRate,
                OverallDifficulty = difficultyAttributes.OverallDifficulty,
                DrainRate = difficultyAttributes.DrainRate,
                HitCircleCount = difficultyAttributes.HitCircleCount,
                SpinnerCount = difficultyAttributes.SpinnerCount
            }));
        }

        public override OsuDifficulty GetDifficulty(object difficultyAttributes)
        {
            var osuDifficultyAttributes = (OsuDifficultyAttributes)difficultyAttributes;
            return new OsuDifficulty()
            {
                Total = osuDifficultyAttributes.StarRating,
                Aim = osuDifficultyAttributes.AimStrain,
                Speed = osuDifficultyAttributes.SpeedStrain,
                Flashlight = osuDifficultyAttributes.FlashlightRating
            };
        }

        public override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<OsuDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        public override OsuPerformance CalculatePerformance(OsuScore score, object difficultyAttributes)
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

            var performanceCalculator = OsuRuleset.CreatePerformanceCalculator((OsuDifficultyAttributes)difficultyAttributes, scoreInfo);
            var categoryAttributes = new Dictionary<string, double>();
            var performance = performanceCalculator.Calculate(categoryAttributes);

            return new OsuPerformance()
            {
                Total = performance,
                Aim = categoryAttributes["Aim"],
                Speed = categoryAttributes["Speed"],
                Accuracy = categoryAttributes["Accuracy"],
                Flashlight = categoryAttributes["Flashlight"]
            };
        }

        public override OsuCalculation GetCalculation(OsuDifficulty difficulty, OsuPerformance performance)
        {
            return new OsuCalculation()
            {
                Difficulty = difficulty,
                Performance = performance
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(int beatmapId)
        {
            var beatmapPath = Path.Combine(_configuration["BEATMAP_DIRECTORY"], beatmapId.ToString());
            return new CalculatorWorkingBeatmap(OsuRuleset, beatmapPath, beatmapId);
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
