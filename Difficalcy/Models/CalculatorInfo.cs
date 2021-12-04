namespace Difficalcy.Models
{
    public record CalculatorInfo
    {
        public string RulesetName { get; init; }
        public string CalculatorName { get; init; }
        public string CalculatorPackage { get; init; }
        public string CalculatorVersion { get; init; }
    }
}
