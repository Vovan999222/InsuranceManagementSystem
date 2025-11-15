using System.Text.Json;

namespace InsuranceManagementSystem.Services
{
    public class ApiIntegrationService
    {
        private readonly HttpClient _httpClient;

        public ApiIntegrationService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        // Метод для отримання курсів валют (приклад)
        public async Task<decimal> GetExchangeRate(string fromCurrency, string toCurrency)
        {
            try
            {
                // В реальному додатку тут буде виклик реального API
                // Для демонстрації повертаємо фіксовані значення
                await Task.Delay(100); // Імітація затримки мережі

                var rates = new Dictionary<string, decimal>
                {
                    { "USD-UAH", 39.50m },
                    { "EUR-UAH", 42.80m },
                    { "USD-EUR", 0.92m }
                };

                var key = $"{fromCurrency}-{toCurrency}";
                if (rates.ContainsKey(key))
                    return rates[key];

                return 1.0m;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання курсу валют: {ex.Message}");
                return 1.0m;
            }
        }

        // Метод для перевірки авто через API (приклад)
        public async Task<CarInfo> GetCarInfo(string licensePlate)
        {
            try
            {
                await Task.Delay(100); // Імітація затримки мережі

                // Повертаємо тестові дані
                return new CarInfo
                {
                    Make = "Toyota",
                    Model = "Camry",
                    Year = 2020,
                    VIN = "JTDBT123456789012"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка отримання інформації про авто: {ex.Message}");
                return new CarInfo { Make = "Невідомо", Model = "Невідомо", Year = DateTime.Now.Year };
            }
        }
    }

    // Допоміжні класи для API
    public class ExchangeRateResponse
    {
        public string Base { get; set; } = string.Empty;
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }

    public class CarInfo
    {
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public string VIN { get; set; } = string.Empty;
    }
}