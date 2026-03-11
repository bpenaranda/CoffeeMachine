using System.Text.Json.Serialization;

namespace CoffeeMachine.Models
{
    public class CoffeeResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("prepared")]
        public string Prepared { get; set; } = string.Empty;
    }
}
