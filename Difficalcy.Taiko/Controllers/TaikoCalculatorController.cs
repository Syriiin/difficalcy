using Difficalcy.Controllers;
using Difficalcy.Taiko.Models;
using Difficalcy.Taiko.Services;

namespace Difficalcy.Taiko.Controllers
{
    public class TaikoCalculatorController : CalculatorController<TaikoScore, TaikoDifficulty, TaikoPerformance, TaikoCalculation, TaikoCalculatorService>
    {
        public TaikoCalculatorController(TaikoCalculatorService calculatorService) : base(calculatorService) { }
    }
}
