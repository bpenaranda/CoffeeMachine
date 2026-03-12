using CoffeeMachine.Interfaces;
using CoffeeMachine.Services;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace CoffeeMachine.Tests
{
    public class CoffeeServiceTests
    {
        [Fact]
        public async Task PrepareCoffeeAsync_AprilFools_Returns418()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 4, 1, 18, 41, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(418, result.StatusCode);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_Normal_Returns200WithAllResponse()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            var testDate = new DateTimeOffset(2026, 2, 14, 13, 30, 0, TimeSpan.Zero);
            fakeTimeProvider.SetUtcNow(testDate);

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Response);
            Assert.Equal("Your piping hot coffee is ready", result.Response.Message);
            Assert.Contains("2026-02-14T13:30:00", result.Response.Prepared);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_5thCoffee_Returns503()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 5, 3, 5, 30, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            for (int i = 0; i < 4; i++)
            {
                await service.PrepareCoffeeAsync();
            }

            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(503, result.StatusCode);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_6thCoffee_Returns200()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 1, 2, 3, 40, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            for (int i = 0; i < 5; i++)
            {
                await service.PrepareCoffeeAsync();
            }

            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Response);
            Assert.Equal("Your piping hot coffee is ready", result.Response.Message);
            Assert.Contains("2026-01-02T03:40:00", result.Response.Prepared);
        }


        [Fact]
        public async Task PrepareCoffeeAsync_10thCoffee_Returns503()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 10, 3, 5, 30, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            for (int i = 0; i < 9; i++)
            {
                await service.PrepareCoffeeAsync();
            }

            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(503, result.StatusCode);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_AprilFoolsAnd5thCoffee_Returns418()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 4, 1, 18, 41, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            for (int i = 0; i < 4; i++)
            {
                await service.PrepareCoffeeAsync();
            }

            var result = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(418, result.StatusCode);
            Assert.Null(result.Response);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_NextDay_Returns418Then503()
        {
            //Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 4, 1, 23, 59, 0, TimeSpan.Zero));

            var service = new CoffeeService(fakeTimeProvider, new Mock<IWeatherService>().Object);

            //Act
            for (int i = 0; i < 4; i++)
            {
                var result = await service.PrepareCoffeeAsync();
                Assert.Equal(418, result.StatusCode);
            }

            fakeTimeProvider.Advance(TimeSpan.FromMinutes(5));

            var fifthResult = await service.PrepareCoffeeAsync();

            //Assert
            Assert.Equal(503, fifthResult.StatusCode);
            Assert.Null(fifthResult.Response);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_TempAbove30_Returns200IcedCoffeeMessage()
        {
            // Arrange
            var fakeTimeProvider = new FakeTimeProvider();
            fakeTimeProvider.SetUtcNow(new DateTimeOffset(2026, 5, 1, 12, 0, 0, TimeSpan.Zero));

            var mockWeather = new Mock<IWeatherService>();
            mockWeather.Setup(w => w.GetTemperatureAsync()).ReturnsAsync(35.0);

            var service = new CoffeeService(fakeTimeProvider, mockWeather.Object);

            // Act
            var result = await service.PrepareCoffeeAsync();

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Response);
            Assert.Equal("Your refreshing iced coffee is ready", result.Response.Message);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_TempBelow31_Returns200HotCoffeeMessage()
        {
            // Arrange
            var fakeTimeProvider = new FakeTimeProvider();

            var mockWeather = new Mock<IWeatherService>();
            mockWeather.Setup(w => w.GetTemperatureAsync()).ReturnsAsync(30.0);

            var service = new CoffeeService(fakeTimeProvider, mockWeather.Object);

            // Act
            var result = await service.PrepareCoffeeAsync();

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result.Response);
            Assert.Equal("Your piping hot coffee is ready", result.Response?.Message);
        }

        [Fact]
        public async Task PrepareCoffeeAsync_WeatherApiFails_DefaultsToHotCoffeeMessage()
        {
            // Arrange
            var fakeTimeProvider = new FakeTimeProvider();

            var mockWeather = new Mock<IWeatherService>();
            mockWeather.Setup(w => w.GetTemperatureAsync()).ReturnsAsync((double?)null);

            var service = new CoffeeService(fakeTimeProvider, mockWeather.Object);

            // Act
            var result = await service.PrepareCoffeeAsync();

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Your piping hot coffee is ready", result.Response?.Message);
        }
    }
}