using CoffeeMachine.Services;
using Microsoft.Extensions.Time.Testing;

namespace CoffeeMachine.Tests
{
    public class CoffeeServiceTests
    {
        [Fact]
        public async Task PrepareCoffeeAsync_Normal_Returns200WithAllResponse()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            var testDate = new DateTimeOffset(2026, 2, 14, 13, 30, 0, TimeSpan.Zero);
            fakeTimeProvider.SetUtcNow(testDate);

            var service = new CoffeeService(fakeTimeProvider);

            //Act
            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Response);
            Assert.Equal("Your piping hot coffee is ready", result.Response.Message);
            Assert.Contains("2026-02-14T13:30:00", result.Response.Prepared);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_FifthCoffee_Returns503()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 5, 3, 5, 30, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider);

            //Act
            for (int i = 0; i < 4; i++)
            {
                await service.PrepareCoffeeAsync();
            }
            ;
            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(503, result.StatusCode);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_AprilFools_Returns418()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 4, 1, 18, 41, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider);

            //Act
            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(418, result.StatusCode);
            Assert.Null(result.Response);
        }
    }
}
