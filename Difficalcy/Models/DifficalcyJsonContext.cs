using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Difficalcy.Models;

[JsonSerializable(typeof(CalculatorInfo))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(Dictionary<string, CalculatorInfo>))]
public partial class DifficalcyJsonContext : JsonSerializerContext { }
