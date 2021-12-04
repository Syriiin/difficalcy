using Difficalcy.Controllers;
using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;

namespace Difficalcy.Mania.Controllers
{
    public class ManiaCalculatorController : CalculatorController<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation, ManiaCalculatorService>
    {
        public ManiaCalculatorController(ManiaCalculatorService calculatorService) : base(calculatorService) { }
    }
}
