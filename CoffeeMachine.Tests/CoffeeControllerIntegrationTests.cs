using CoffeeMachine.Interfaces;
using CoffeeMachine.Models;
using CoffeeMachine.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using System.Net;
using System.Text.Json;

namespace CoffeeMachine.Tests
{
    public class CoffeeControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CoffeeControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_BrewCoffee_Returns200()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var result = await client.GetAsync("/brew-coffee");

            //Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var responseString = await result.Content.ReadAsStringAsync();
            var coffeeResponse = JsonSerializer.Deserialize<CoffeeResponse>(
                responseString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(coffeeResponse);
            Assert.Equal("Your piping hot coffee is ready", coffeeResponse.Message);
            Assert.NotEmpty(coffeeResponse.Prepared);
        }

        [Fact]
        public async Task Get_April1st_Returns418()
        {
            //Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var time = new FakeTimeProvider();
                    time.SetUtcNow(new DateTimeOffset(2026,4,1,4,10,0,TimeSpan.Zero));

                    services.AddSingleton<TimeProvider>(time);
                });
            }).CreateClient();

            //Act
            var result = await client.GetAsync("/brew-coffee");

            //Assert
            Assert.Equal((HttpStatusCode)418, result.StatusCode);
        }

        [Fact]
        public async Task Get_5thCoffeee_Returns508()
        {
            //Arrange
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var time = new FakeTimeProvider();
                    time.SetUtcNow(new DateTimeOffset(2026, 3, 12, 8, 0, 0, TimeSpan.Zero));

                    services.AddSingleton<TimeProvider>(time);
                    services.AddSingleton<ICoffeeService, CoffeeService>();
                });
            }).CreateClient();

            //Act
            //Assert
            for (int i = 0; i < 4;  i++)
            {
                var result = await client.GetAsync("/brew-coffee");
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            }

            var response = await client.GetAsync("/brew-coffee");

            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}
