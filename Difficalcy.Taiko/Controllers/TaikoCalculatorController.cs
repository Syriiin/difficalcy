using Difficalcy.Controllers;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;

namespace Difficalcy.Taiko.Controllers
{
    public class TaikoCalculatorController(TaikoCalculatorService calculatorService)
        : CalculatorController<
            TaikoScore,
            TaikoDifficulty,
            TaikoPerformance,
            TaikoCalculation,
            TaikoBeatmapDetails,
            TaikoCalculatorService
        >(calculatorService) { }
}
