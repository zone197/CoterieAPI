using System.Net;
using Microsoft.AspNetCore.Mvc;
using MiniRater.Interfaces;
using MiniRater.Models;

namespace Coterie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MiniRaterController : ControllerBase
    {
        private readonly IRateCalculatorService _rateCalculatorService;
        private readonly ILogger<MiniRaterController> _logger;
        public MiniRaterController(IRateCalculatorService rateCalculatorService, ILogger<MiniRaterController> logger)
        {
            _rateCalculatorService = rateCalculatorService;
            _logger = logger;
        }

        [HttpPost("GetRates")]
        public async Task<IActionResult> GetRates([FromBody] RateCalculatorRequestModel request)
        {
            try
            {
                if (request == null)
                {
                   _logger.LogError("Request is null");
                    return BadRequest("Request cannot be null.");
                }
                var result = await _rateCalculatorService.CalculatePremiumAsync(request);
                if (!result.IsSuccessful)
                {
                    _logger.LogError($"Error calculating premium for business: {request.Business}");
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Internal server error");
            }
           
        
    }
}
}
