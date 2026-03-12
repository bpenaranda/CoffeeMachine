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
                return (StatusCodes.Status418ImATeapot, null);

            if (currentCount % 5 == 0)
                return (StatusCodes.Status503ServiceUnavailable, null);

            var temp = await _weatherService.GetTemperatureAsync();
            string message = temp.HasValue && temp.Value > 30
                ? "Your refreshing iced coffee is ready"
                : "Your piping hot coffee is ready";

            return (StatusCodes.Status200OK, new CoffeeResponse
            {
                Message = message,
                Prepared = now.ToString("yyyy-MM-ddTHH:mm:ssK")
            });
        }
    }
}
