using CoffeeMachine.Interfaces;
using CoffeeMachine.Models;

namespace CoffeeMachine.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly TimeProvider _timeProvider;
        private readonly IWeatherService _weatherService;
        private int _callCounter = 0;

        public CoffeeService(TimeProvider timeProvider, IWeatherService weatherService)
        {
            _timeProvider = timeProvider;
            _weatherService = weatherService;
        }

        public async Task<(int StatusCode, CoffeeResponse? Response)> PrepareCoffeeAsync()
        {
            var now = _timeProvider.GetLocalNow();
            int currentCount = Interlocked.Increment(ref _callCounter);

            if (now.Month == 4 && now.Day == 1)
                return (418, null);

            if (currentCount % 5 == 0)
                return (503, null);

            var temp = await _weatherService.GetTemperatureAsync();
            string message = temp.HasValue && temp.Value > 30
                ? "Your refreshing iced coffee is ready"
                : "Your piping hot coffee is ready";

            return (200, new CoffeeResponse
            {
                Message = message,
                Prepared = now.ToString("yyyy-MM-ddTHH:mm:ssK")
            });
        }
    }
}
