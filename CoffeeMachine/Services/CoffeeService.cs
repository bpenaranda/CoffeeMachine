using CoffeeMachine.Interfaces;
using CoffeeMachine.Models;

namespace CoffeeMachine.Services
{
    public class CoffeeService : ICoffeeService
    {
        private readonly TimeProvider _timeProvider;
        private int _callCounter = 0;

        public CoffeeService(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public async Task<(int StatusCode, CoffeeResponse? Response)> PrepareCoffeeAsync()
        {
            if (Interlocked.Increment(ref _callCounter) % 5 == 0)
                return (503, null);

            var now = _timeProvider.GetLocalNow().DateTime;
            if (now.Month == 4 && now.Day == 1)
                return (418, null);

            return (200, new CoffeeResponse
            {
                Message = "Your piping hot coffee is ready",
                Prepared = now.ToString("yyyy-MM-ddTHH:mm:sszzz")
            });
        }
    }
}
