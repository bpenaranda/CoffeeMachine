using System.Text.Json.Serialization;

namespace CoffeeMachine.Models
{
    public class WeatherResponse
    {
        [JsonPropertyName("main")]
        public TemperatureData Main { get; set; } = new TemperatureData();
    }

    public class TemperatureData
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }
    }
}
