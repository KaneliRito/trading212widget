using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Trading212Stick.Models;

namespace Trading212Stick.Services
{
    public class Trading212Service
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigurationService _configService;

        public Trading212Service(ConfigurationService configService)
        {
            _configService = configService;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<PortfolioInfo> GetPortfolioDataAsync()
        {
            var portfolioInfo = new PortfolioInfo();

            try
            {
                var apiUrl = _configService.Settings.Trading212.ApiUrl;
                var apiKey = _configService.Settings.Trading212.ApiKey;
                var apiSecret = _configService.Settings.Trading212.ApiSecret;

                if (string.IsNullOrWhiteSpace(apiUrl) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
                {
                    portfolioInfo.IsError = true;
                    portfolioInfo.ErrorMessage = "API URL, API Key of API Secret niet geconfigureerd";
                    return portfolioInfo;
                }

                // Basic Authentication: Base64(API_KEY:API_SECRET)
                // Docs: "The Authorization header is constructed by Base64-encoding your API_KEY:API_SECRET string and prepending it with Basic ."
                var credentials = $"{apiKey}:{apiSecret}";
                var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", $"Basic {base64Credentials}");
                request.Headers.Add("User-Agent", "Trading212Stick/1.0");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorMessage = $"API Error: {response.StatusCode}";
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        errorMessage += " - Toegang geweigerd. Controleer of je API Key de 'Account Data' permissie heeft en of je een Invest/ISA account hebt (geen CFD).";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        errorMessage += " - Authenticatie mislukt. Controleer je API Key en Secret.";
                    }
                    else if ((int)response.StatusCode == 429) // Too Many Requests
                    {
                        errorMessage += " - Te veel verzoeken. Wacht even voordat de data wordt ververs (limiet bereikt).";
                        // Optioneel: check headers voor reset tijd
                        if (response.Headers.TryGetValues("x-ratelimit-reset", out var resetUnixValues))
                        {
                            // we could parse this but keeping it simple for now
                        }
                    }
                    
                    if (!string.IsNullOrWhiteSpace(errorContent))
                    {
                        errorMessage += $" Details: {errorContent}";
                    }

                    portfolioInfo.IsError = true;
                    portfolioInfo.ErrorMessage = errorMessage;
                    return portfolioInfo;
                }

                var content = await response.Content.ReadAsStringAsync();
                var apiData = JsonConvert.DeserializeObject<Trading212Portfolio>(content);

                if (apiData != null)
                {
                    portfolioInfo.TotalValue = apiData.TotalValue;
                    
                    // Bereken dagresultaat: huidige waarde - totale kosten
                    var totalInvested = apiData.Investments.TotalCost;
                    var currentValue = apiData.Investments.CurrentValue;
                    portfolioInfo.DayResult = apiData.Investments.UnrealizedProfitLoss; // Dit is de huidige unrealized P/L
                    portfolioInfo.DayResultPercentage = totalInvested > 0 
                        ? (apiData.Investments.UnrealizedProfitLoss / totalInvested) * 100 
                        : 0;
                    
                    portfolioInfo.TotalProfitLoss = apiData.Investments.UnrealizedProfitLoss + apiData.Investments.RealizedProfitLoss;
                    portfolioInfo.TotalProfitLossPercentage = totalInvested > 0 
                        ? (portfolioInfo.TotalProfitLoss / totalInvested) * 100 
                        : 0;
                    
                    portfolioInfo.LastUpdated = DateTime.Now;
                    portfolioInfo.IsError = false;
                }
            }
            catch (HttpRequestException ex)
            {
                portfolioInfo.IsError = true;
                portfolioInfo.ErrorMessage = $"Netwerkfout: {ex.Message}";
            }
            catch (TaskCanceledException)
            {
                portfolioInfo.IsError = true;
                portfolioInfo.ErrorMessage = "Request timeout";
            }
            catch (Exception ex)
            {
                portfolioInfo.IsError = true;
                portfolioInfo.ErrorMessage = $"Fout: {ex.Message}";
            }

            return portfolioInfo;
        }
    }
}
