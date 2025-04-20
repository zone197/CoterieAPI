using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniRater.Interfaces;
using MiniRater.Models;

namespace MiniRater.Services
{
    public class RateCalculatorService : IRateCalculatorService
    {
        private readonly RateConfig _config;
        private readonly ILogger<RateCalculatorService> _logger;
        public RateCalculatorService(IOptions<RateConfig> config, ILogger<RateCalculatorService> logger)
        {
            _config = config.Value;
            _logger = logger;

        }
        public async Task<RateCalculatorResponseModel> CalculatePremiumAsync(RateCalculatorRequestModel request)
        {
            try
            {
               
                var transactionId = Guid.NewGuid().ToString();
                var response = new RateCalculatorResponseModel
                {
                    Premiums = new List<PremiumInfo>(),
                    TransactionId = transactionId
                };
                if (request.Revenue > 0) 
                {

                    if (!_config.Business.Contains(request.Business.ToUpper()))
                        {
                            _logger.LogError($"Incorrect business type: {request.Business}");
                            response.IsSuccessful = false;
                            return await Task.FromResult(response);
                        }
                        var statesAbbr = new List<string>();
                        foreach (var state in request.States)
                        {
                            var abbrState = formatState(state);
                            if (!string.IsNullOrEmpty(abbrState) || _config.States.Contains(abbrState))
                            {
                                statesAbbr.Add(abbrState);
                            }

                        }

                        var basePremium = _config.BasePremium;
                        var businessFactor = _config.BusinessFactors[request.Business.ToUpper()];
                        foreach (var state in statesAbbr)
                        {
                            var stateFactor = _config.StateFactors[state];
                            var hazardFactor = _config.HazardFactor;
                            var premium = Math.Round((request.Revenue / basePremium) * stateFactor * businessFactor * hazardFactor);
                            response.Premiums.Add(new PremiumInfo
                            {
                                Premium = Math.Round(premium, 2),
                                State = state
                            });
                        }

                        if (response.Premiums.Count == 0)
                        {
                            _logger.LogWarning($"No Premium Calculated for : {request.Business}");
                            response.IsSuccessful = false;
                            return await Task.FromResult(response);
                        }

                         response.Business = request.Business;
                         response.Revenue = request.Revenue;
                         response.IsSuccessful = true;
                }
                    else
                    {
                        _logger.LogError("Revenue is required");
                        response.IsSuccessful = false;
                    }
                        return await Task.FromResult(response);

                 }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating premium.");
                throw;
            }
        }

        private string formatState(string state)
        {
            if (state.Length == 2)
                return state.ToUpper();

            var abbrState =  _config.StateAbbr.TryGetValue(state.ToUpper(), out var abbr)
            ? abbr.ToUpperInvariant()
            : string.Empty;

            return abbrState;
        }
    }
    
}
