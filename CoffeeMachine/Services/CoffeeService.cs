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
            var now = _timeProvider.GetLocalNow();
            int currentCount = Interlocked.Increment(ref _callCounter);

            if (now.Month == 4 && now.Day == 1)
                return (StatusCodes.Status418ImATeapot, null);

            if (currentCount % 5 == 0)
                return (StatusCodes.Status503ServiceUnavailable, null);

            return (StatusCodes.Status200OK, new CoffeeResponse
            {
                Message = "Your piping hot coffee is ready",
                Prepared = now.ToString("yyyy-MM-ddTHH:mm:ssK")
            });
        }
    }
}
