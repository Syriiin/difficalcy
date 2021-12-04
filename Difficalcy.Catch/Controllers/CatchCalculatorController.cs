using Difficalcy.Controllers;
using Difficalcy.Catch.Models;
using Difficalcy.Catch.Services;

namespace Difficalcy.Catch.Controllers
{
    public class CatchCalculatorController : CalculatorController<CatchScore, CatchDifficulty, CatchPerformance, CatchCalculation, CatchCalculatorService>
    {
        public CatchCalculatorController(CatchCalculatorService calculatorService) : base(calculatorService) { }
    }
}
