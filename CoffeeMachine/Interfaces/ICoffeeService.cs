using CoffeeMachine.Models;

namespace CoffeeMachine.Interfaces
{
    public interface ICoffeeService
    {
        Task<(int StatusCode, CoffeeResponse? Response)> PrepareCoffeeAsync();
    }
}
