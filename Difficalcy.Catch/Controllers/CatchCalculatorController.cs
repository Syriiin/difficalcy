using Difficalcy.Controllers;
using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;

namespace Difficalcy.Catch.Controllers
{
    public class CatchCalculatorController(CatchCalculatorService calculatorService) : CalculatorController<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation, CatchCalculatorService>(calculatorService)
    {
    }
}
