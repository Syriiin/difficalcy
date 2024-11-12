using Difficalcy.Controllers;
using Difficalcy.Osu.Models;
using Difficalcy.Osu.Services;

namespace Difficalcy.Osu.Controllers
{
    public class OsuCalculatorController(OsuCalculatorService calculatorService)
        : CalculatorController<
            OsuScore,
            OsuDifficulty,
            OsuPerformance,
            OsuCalculation,
            OsuCalculatorService
        >(calculatorService) { }
}
