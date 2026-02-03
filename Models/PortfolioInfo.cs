using System;

namespace Trading212Stick.Models
{
    public class PortfolioInfo
    {
        public decimal TotalValue { get; set; }
        public decimal DayResult { get; set; }
        public decimal DayResultPercentage { get; set; }
        public decimal TotalProfitLoss { get; set; }
        public decimal TotalProfitLossPercentage { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Currency { get; set; } = "EUR";
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }

        public PortfolioInfo()
        {
            LastUpdated = DateTime.Now;
        }
    }
}
