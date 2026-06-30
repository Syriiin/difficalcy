using System.Text.Json.Serialization;
using Difficalcy.Models;
using osu.Game.Rulesets.Mania.Difficulty;

namespace Difficalcy.Mania.Models;

// Response models
[JsonSerializable(typeof(ManiaCalculation))]
[JsonSerializable(typeof(ManiaCalculation[]))]
[JsonSerializable(typeof(ManiaBeatmapDetails))]
// Request models
[JsonSerializable(typeof(Mod), TypeInfoPropertyName = "ScoreMod")]
[JsonSerializable(typeof(Mod[]), TypeInfoPropertyName = "ScoreModArray")]
[JsonSerializable(typeof(ManiaScore))]
[JsonSerializable(typeof(ManiaScore[]))]
// Internal models
[JsonSerializable(typeof(ManiaDifficultyAttributes))]
[JsonSerializable(typeof(ManiaDifficultyAttributesDto))]
public partial class ManiaJsonContext : JsonSerializerContext { }
