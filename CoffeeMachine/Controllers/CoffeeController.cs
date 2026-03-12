using CoffeeMachine.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeMachine.Controllers
{
    [ApiController]
    public class CoffeeController : ControllerBase
    {
        private readonly ICoffeeService _coffeeService;

        public CoffeeController(ICoffeeService coffeeService)
        {
            _coffeeService = coffeeService;
        }

        [HttpGet("/brew-coffee")]
        public async Task<IActionResult> GetCoffee()
        {
            var (statusCode, response) = await _coffeeService.PrepareCoffeeAsync();

            if (statusCode == StatusCodes.Status200OK)
            {
                return Ok(response);
            }

            return StatusCode(statusCode);
        }
    }
}
