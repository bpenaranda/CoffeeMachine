using CoffeeMachine.Interfaces;
using CoffeeMachine.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ICoffeeService, CoffeeService>();

builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    var baseUrl = builder.Configuration["WeatherAPI:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = string.Empty;
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coffee Machine");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
