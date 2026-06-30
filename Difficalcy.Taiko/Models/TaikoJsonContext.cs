using System.Text.Json.Serialization;
using Difficalcy.Models;
using osu.Game.Rulesets.Taiko.Difficulty;

namespace Difficalcy.Taiko.Models;

// Response models
[JsonSerializable(typeof(TaikoCalculation))]
[JsonSerializable(typeof(TaikoCalculation[]))]
[JsonSerializable(typeof(TaikoBeatmapDetails))]
// Request models
[JsonSerializable(typeof(Mod), TypeInfoPropertyName = "ScoreMod")]
[JsonSerializable(typeof(Mod[]), TypeInfoPropertyName = "ScoreModArray")]
[JsonSerializable(typeof(TaikoScore))]
[JsonSerializable(typeof(TaikoScore[]))]
// Internal models
[JsonSerializable(typeof(TaikoDifficultyAttributes))]
[JsonSerializable(typeof(TaikoDifficultyAttributesDto))]
public partial class TaikoJsonContext : JsonSerializerContext { }
