using System.Linq;
using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Difficalcy.Controllers
{
    [ApiController]
    [Route("/api")]
    [Produces("application/json")]
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

        /// <summary>
        /// Returns a set of information describing the calculator.
        /// </summary>
        [HttpGet("info")]
        public ActionResult<CalculatorInfo> GetInfo()
        {
            return Ok(calculatorService.Info);
        }

        /// <summary>
        /// Returns difficulty values for a score.
        /// </summary>
        [HttpGet("difficulty")]
        public async Task<ActionResult<TDifficulty>> GetDifficulty([FromQuery] TScore score)
        {
            return Ok(await calculatorService.GetDifficulty(score));
        }

        /// <summary>
        /// Returns performance values for a score.
        /// </summary>
        [HttpGet("performance")]
        public async Task<ActionResult<TPerformance>> GetPerformance([FromQuery] TScore score)
        {
            return Ok(await calculatorService.GetPerformance(score));
        }

        /// <summary>
        /// Returns difficulty and performance values for a score.
        /// </summary>
        [HttpGet("calculation")]
        public async Task<ActionResult<TCalculation>> GetCalculation([FromQuery] TScore score)
        {
            return Ok(await calculatorService.GetCalculation(score));
        }

        /// <summary>
        /// Returns difficulty values for a batch of scores.
        /// </summary>
        [HttpPost("batch/difficulty")]
        [Consumes("application/json")]
        public async Task<ActionResult<TDifficulty[]>> GetDifficultyBatch([FromBody] TScore[] scores)
        {
            return Ok(await Task.WhenAll(scores.Select(score => calculatorService.GetDifficulty(score))));
        }

        /// <summary>
        /// Returns performance values for a batch of scores.
        /// </summary>
        [HttpPost("batch/performance")]
        [Consumes("application/json")]
        public async Task<ActionResult<TPerformance[]>> GetPerformanceBatch([FromBody] TScore[] scores)
        {
            return Ok(await Task.WhenAll(scores.Select(score => calculatorService.GetPerformance(score))));
        }

        /// <summary>
        /// Returns difficulty and performance values for a batch of scores.
        /// </summary>
        [HttpPost("batch/calculation")]
        [Consumes("application/json")]
        public async Task<ActionResult<TCalculation[]>> GetCalculationBatch([FromBody] TScore[] scores)
        {
            return Ok(await Task.WhenAll(scores.Select(score => calculatorService.GetCalculation(score))));
        }
    }
}
