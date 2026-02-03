using Newtonsoft.Json;

namespace Trading212Stick.Models
{
    // Response model voor Trading 212 Account Summary API
    public class Trading212Portfolio
    {
        [JsonProperty("totalValue")]
        public decimal TotalValue { get; set; }

        [JsonProperty("investments")]
        public InvestmentData Investments { get; set; } = new();

        [JsonProperty("cash")]
        public CashData Cash { get; set; } = new();
    }

    public class InvestmentData
    {
        [JsonProperty("currentValue")]
        public decimal CurrentValue { get; set; }

        [JsonProperty("totalCost")]
        public decimal TotalCost { get; set; }

        [JsonProperty("unrealizedProfitLoss")]
        public decimal UnrealizedProfitLoss { get; set; }

        [JsonProperty("realizedProfitLoss")]
        public decimal RealizedProfitLoss { get; set; }
    }

    public class CashData
    {
        [JsonProperty("availableToTrade")]
        public decimal AvailableToTrade { get; set; }

        [JsonProperty("reservedForOrders")]
        public decimal ReservedForOrders { get; set; }

        [JsonProperty("inPies")]
        public decimal InPies { get; set; }
    }
}
