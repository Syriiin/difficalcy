using System.Threading.Tasks;
using Difficalcy.Models;
using Difficalcy.Services;
using Microsoft.AspNetCore.Mvc;

namespace Difficalcy.Controllers
{
    [ApiController]
    [Route("/api")]
    [Produces("application/json")]
    public abstract class CalculatorController<
        TScore,
        TDifficulty,
        TPerformance,
        TCalculation,
        TBeatmapDetails,
        TCalculatorService
    >(TCalculatorService calculatorService) : ControllerBase
        where TScore : Score
        where TDifficulty : Difficulty
        where TPerformance : Performance
        where TCalculation : Calculation<TDifficulty, TPerformance>
        where TBeatmapDetails : BeatmapDetails
        where TCalculatorService : CalculatorService<
                TScore,
                TDifficulty,
                TPerformance,
                TCalculation,
                TBeatmapDetails
            >
    {
        protected readonly TCalculatorService calculatorService = calculatorService;

        /// <summary>
        /// Returns a set of information describing the calculator.
        /// </summary>
        [HttpGet("info")]
        public ActionResult<CalculatorInfo> GetInfo()
        {
            return Ok(calculatorService.Info);
        }

        /// <summary>
        /// Returns beatmap details.
        /// </summary>
        [HttpGet("beatmapdetails")]
        public async Task<ActionResult<TBeatmapDetails>> GetBeatmapDetails(
            [FromQuery] string beatmapId
        )
        {
            try
            {
                return Ok(await calculatorService.GetBeatmapDetails(beatmapId));
            }
            catch (BeatmapNotFoundException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        /// <summary>
        /// Returns difficulty and performance values for a score.
        /// </summary>
        [HttpGet("calculation")]
        public async Task<ActionResult<TCalculation>> GetCalculation([FromQuery] TScore score)
        {
            try
            {
                return Ok(await calculatorService.GetCalculation(score));
            }
            catch (BeatmapNotFoundException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        /// <summary>
        /// Returns difficulty and performance values for a batch of scores.
        /// </summary>
        [HttpPost("batch/calculation")]
        [Consumes("application/json")]
        public async Task<ActionResult<TCalculation[]>> GetCalculationBatch(
            [FromBody] TScore[] scores
        )
        {
            try
            {
                return Ok(await calculatorService.GetCalculationBatch(scores));
            }
            catch (BeatmapNotFoundException e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
