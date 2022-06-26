using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Difficalcy.Taiko.Models;
using Microsoft.Extensions.Configuration;
using osu.Game.Beatmaps.Legacy;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.Taiko;
using osu.Game.Rulesets.Taiko.Difficulty;
using osu.Game.Rulesets.Taiko.Objects;
using osu.Game.Scoring;

namespace Difficalcy.Taiko.Services
{
    public class TaikoCalculatorService : CalculatorService<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation>
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient = new HttpClient();
        private TaikoRuleset TaikoRuleset { get; } = new TaikoRuleset();

        public override CalculatorInfo Info
        {
            get
            {
                var packageName = Assembly.GetAssembly(typeof(TaikoRuleset)).GetName().Name;
                var packageVersion = Assembly.GetAssembly(typeof(TaikoRuleset)).GetName().Version.ToString();
                return new CalculatorInfo
                {
                    RulesetName = TaikoRuleset.Description,
                    CalculatorName = "Official osu!taiko",
                    CalculatorPackage = packageName,
                    CalculatorVersion = packageVersion,
                    CalculatorUrl = $"https://nuget.org/packages/ppy.{packageName}/{packageVersion}"
                };
            }
        }

        public TaikoCalculatorService(IConfiguration configuration, ICache cache) : base(cache)
        {
            _configuration = configuration;
        }

        public override async Task EnsureBeatmap(int beatmapId)
        {
            var beatmapPath = Path.Combine(_configuration["BEATMAP_DIRECTORY"], beatmapId.ToString());
            if (!File.Exists(beatmapPath))
            {
                var response = await _httpClient.GetStreamAsync($"https://osu.ppy.sh/osu/{beatmapId}");
                using (var fs = new FileStream(beatmapPath, FileMode.CreateNew))
                    await response.CopyToAsync(fs);
            }
        }

        public override (object, string) CalculateDifficulty(TaikoScore score)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = TaikoRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();

            var difficultyCalculator = TaikoRuleset.CreateDifficultyCalculator(workingBeatmap);
            var difficultyAttributes = difficultyCalculator.Calculate(mods) as TaikoDifficultyAttributes;

            // Serialising anonymous object with same names because Mods and Skills can't be serialised
            return (difficultyAttributes, JsonSerializer.Serialize(new
            {
                StarRating = difficultyAttributes.StarRating,
                MaxCombo = difficultyAttributes.MaxCombo,
                StaminaStrain = difficultyAttributes.StaminaStrain,
                RhythmStrain = difficultyAttributes.RhythmStrain,
                ColourStrain = difficultyAttributes.ColourStrain,
                ApproachRate = difficultyAttributes.ApproachRate,
                GreatHitWindow = difficultyAttributes.GreatHitWindow
            }));
        }

        public override TaikoDifficulty GetDifficultyFromDifficultyAttributes(object difficultyAttributes)
        {
            var taikoDifficultyAttributes = (TaikoDifficultyAttributes)difficultyAttributes;
            return new TaikoDifficulty()
            {
                Total = taikoDifficultyAttributes.StarRating,
                Stamina = taikoDifficultyAttributes.StaminaStrain,
                Rhythm = taikoDifficultyAttributes.RhythmStrain,
                Colour = taikoDifficultyAttributes.ColourStrain
            };
        }

        public override object DeserialiseDifficultyAttributes(string difficultyAttributesJson)
        {
            return JsonSerializer.Deserialize<TaikoDifficultyAttributes>(difficultyAttributesJson, new JsonSerializerOptions() { IncludeFields = true });
        }

        public override TaikoPerformance CalculatePerformance(TaikoScore score, object difficultyAttributes)
        {
            var workingBeatmap = getWorkingBeatmap(score.BeatmapId);
            var mods = TaikoRuleset.ConvertFromLegacyMods((LegacyMods)(score.Mods ?? 0)).ToArray();
            var beatmap = workingBeatmap.GetPlayableBeatmap(TaikoRuleset.RulesetInfo, mods);

            var hitResultCount = beatmap.HitObjects.OfType<Hit>().Count();
            var combo = score.Combo ?? hitResultCount;
            var statistics = determineHitResults(score.Accuracy ?? 1, hitResultCount, score.Misses ?? 0, score.Oks);
            var accuracy = score.Accuracy ?? calculateAccuracy(statistics);

            var scoreInfo = new ScoreInfo()
            {
                Accuracy = accuracy,
                MaxCombo = combo,
                Statistics = statistics,
                Mods = mods
            };

            var performanceCalculator = TaikoRuleset.CreatePerformanceCalculator((TaikoDifficultyAttributes)difficultyAttributes, scoreInfo);
            var categoryAttributes = new Dictionary<string, double>();
            var performance = performanceCalculator.Calculate(categoryAttributes);

            return new TaikoPerformance()
            {
                Total = performance,
                Strain = categoryAttributes["Strain"],
                Accuracy = categoryAttributes["Accuracy"]
            };
        }

        public override TaikoCalculation GetCalculation(TaikoDifficulty difficulty, TaikoPerformance performance)
        {
            return new TaikoCalculation()
            {
                Difficulty = difficulty,
                Performance = performance
            };
        }

        private CalculatorWorkingBeatmap getWorkingBeatmap(int beatmapId)
        {
            var beatmapPath = Path.Combine(_configuration["BEATMAP_DIRECTORY"], beatmapId.ToString());
            return new CalculatorWorkingBeatmap(TaikoRuleset, beatmapPath, beatmapId);
        }

        private Dictionary<HitResult, int> determineHitResults(double targetAccuracy, int hitResultCount, int countMiss, int? countOk)
        {
            // Adapted from https://github.com/ppy/osu-tools/blob/cf5410b04f4e2d1ed2c50c7263f98c8fc5f928ab/PerformanceCalculator/Simulate/TaikoSimulateCommand.cs#L53-L79
            int countGreat;

            if (countOk != null)
            {
                countGreat = (int)(hitResultCount - countOk - countMiss);
            }
            else
            {
                // Let Great=2, Good=1, Miss=0. The total should be this.
                var targetTotal = (int)Math.Round(targetAccuracy * hitResultCount * 2);

                countGreat = targetTotal - (hitResultCount - countMiss);
                countOk = hitResultCount - countGreat - countMiss;
            }

            return new Dictionary<HitResult, int>
            {
                { HitResult.Great, countGreat },
                { HitResult.Ok, (int)countOk },
                { HitResult.Meh, 0 },
                { HitResult.Miss, countMiss }
            };
        }

        private double calculateAccuracy(Dictionary<HitResult, int> statistics)
        {
            var countGreat = statistics[HitResult.Great];
            var countOk = statistics[HitResult.Ok];
            var countMiss = statistics[HitResult.Miss];
            var total = countGreat + countOk + countMiss;

            return (double)((2 * countGreat) + countOk) / (2 * total);
        }
    }
}
