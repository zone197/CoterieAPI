namespace MiniRater.Models
{
    public class RateCalculatorRequestModel
    {
        public double Revenue { get; set; }
        public string Business { get; set; }
        public List<string> States { get; set; }
       
    }
  
    public class RateCalculatorResponseModel
    {
        public string Business { get; set; }
        public double Revenue { get; set; }
        public List<PremiumInfo> Premiums { get; set; }
        public bool IsSuccessful { get; set; }
        public string TransactionId { get; set; }
    }

    public class PremiumInfo
    {
        public double Premium { get; set; }
        public string State { get; set; }
    }

    public class RateConfig
    {
        public double BasePremium { get; set; }
        public double HazardFactor { get; set; }
        public Dictionary<string, double> BusinessFactors { get; set; }
        public Dictionary<string, double> StateFactors { get; set; }
        public List<string> Business { get; set; }
        public List<string> States { get; set; }
        public Dictionary<string, string> StateAbbr { get; set; } 
    }

}



