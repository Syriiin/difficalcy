using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;
using Difficalcy.Controllers;

namespace Difficalcy.Catch.Controllers
{
    public class CatchCalculatorController(CatchCalculatorService calculatorService)
        : CalculatorController<
            CatchScore,
            CatchDifficulty,
            CatchPerformance,
            CatchCalculation,
            CatchCalculatorService
        >(calculatorService) { }
}
