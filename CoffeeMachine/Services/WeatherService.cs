using CoffeeMachine.Interfaces;
using CoffeeMachine.Models;
using System.Text.Json;

namespace CoffeeMachine.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<double?> GetTemperatureAsync()
        {
            try
            {
                var apiKey = _config["WeatherAPI:APIKey"];
                var city = _config["WeatherAPI:City"];

                var url = $"weather?q={city}&appid={apiKey}&units=metric";

                var result = await _httpClient.GetAsync(url);

                if (!result.IsSuccessStatusCode) return null;

                var content = await result.Content.ReadAsStringAsync();
                var weatherData = JsonSerializer.Deserialize<WeatherResponse>(content);

                return weatherData?.Main?.Temp;
            }
            catch {
                return null;
            }
        }
    }
}
