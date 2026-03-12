namespace CoffeeMachine.Interfaces
{
    public interface IWeatherService
    {
        Task<double?> GetTemperatureAsync();
    }
}
