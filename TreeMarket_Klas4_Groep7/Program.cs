using Microsoft.OpenApi.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddAuthorization();

// === add this line to register your DbContext for SQL Server ===
builder.Services.AddDbContext<AppDb>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (HttpContext httpContext) =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.MapControllers();

// Add db-ping endpoint
app.MapGet("/db-ping", async () =>
{
    var cs = app.Configuration.GetConnectionString("Sql");
    await using var c = new SqlConnection(cs);
    await c.OpenAsync();
    await using var cmd = new SqlCommand("SELECT DB_NAME()", c);
    var db = (string)await cmd.ExecuteScalarAsync();
    return Results.Ok(new { connected = true, database = db });
});

app.Run();