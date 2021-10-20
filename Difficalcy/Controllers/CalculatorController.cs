using System.Linq;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Difficalcy.Controllers
{
    [ApiController]
    [Route("/api/calculator")]
    public abstract class CalculatorController<TScore, TDifficulty, TPerformance, TCalculation, TCalculatorService> : ControllerBase
        where TScore : Score
        where TDifficulty : Difficulty
        where TPerformance : Performance
        where TCalculation : Calculation<TDifficulty, TPerformance>
        where TCalculatorService : CalculatorService<TScore, TDifficulty, TPerformance, TCalculation>
    {
        protected readonly TCalculatorService calculatorService;

        public CalculatorController(TCalculatorService calculatorService)
        {
            this.calculatorService = calculatorService;
        }

        [HttpGet("difficulty")]
        public async Task<ActionResult<TDifficulty>> GetDifficulty([FromQuery] TScore score)
        {
            return Ok(await calculatorService.GetDifficulty(score));
        }

        [HttpGet("performance")]
        public async Task<ActionResult<TPerformance>> GetPerformance([FromQuery] TScore score)
        {
            return Ok(await calculatorService.GetPerformance(score));
        }

        [HttpGet("calculation")]
        public async Task<ActionResult<TCalculation>> GetCalculation([FromQuery] TScore score)
        {
            return Ok(await calculatorService.GetCalculation(score));
        }

        [HttpPost("batch/difficulty")]
        public async Task<ActionResult<TDifficulty[]>> GetDifficultyBatch([FromBody] TScore[] scores)
        {
            return Ok(await Task.WhenAll(scores.Select(score => calculatorService.GetDifficulty(score))));
        }

        [HttpPost("batch/performance")]
        public async Task<ActionResult<TPerformance[]>> GetPerformanceBatch([FromBody] TScore[] scores)
        {
            return Ok(await Task.WhenAll(scores.Select(score => calculatorService.GetPerformance(score))));
        }

        [HttpPost("batch/calculation")]
        public async Task<ActionResult<TCalculation[]>> GetCalculationBatch([FromBody] TScore[] scores)
        {
            return Ok(await Task.WhenAll(scores.Select(score => calculatorService.GetCalculation(score))));
        }
    }
}
