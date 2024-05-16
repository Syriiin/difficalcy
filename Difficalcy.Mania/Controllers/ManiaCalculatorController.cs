using Difficalcy.Controllers;
using Difficalcy.Mania.Models;
using Difficalcy.Mania.Services;

namespace Difficalcy.Mania.Controllers
{
    public class ManiaCalculatorController(ManiaCalculatorService calculatorService) : CalculatorController<ManiaScore, ManiaDifficulty, ManiaPerformance, ManiaCalculation, ManiaCalculatorService>(calculatorService)
    {
    }
}
