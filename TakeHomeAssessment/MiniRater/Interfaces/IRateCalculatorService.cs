using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiniRater.Models;

namespace MiniRater.Interfaces
{
    public interface IRateCalculatorService
    {
        public Task<RateCalculatorResponseModel> CalculatePremiumAsync(RateCalculatorRequestModel request);
    }
}
