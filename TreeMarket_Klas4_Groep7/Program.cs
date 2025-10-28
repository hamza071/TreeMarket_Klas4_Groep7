using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TreeMarket_Klas4_Groep7.Data;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Register MVC with views (required for Controller.View, TempData, Razor)
builder.Services.AddControllersWithViews();

// If you want Razor to search the Front-End folder for views:
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // {1} = controller name, {0} = view name
    options.ViewLocationFormats.Add("/Front-End/{1}/{0}.cshtml");
    options.ViewLocationFormats.Add("/Front-End/{0}.cshtml");
});

builder.Services.AddRouting();
builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TreeMarket API",
        Version = "v1"
    });
});

// EF Core
builder.Services.AddDbContext<BloggingContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalExpress")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TreeMarket API v1");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.Run();










//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using TreeMarket_Klas4_Groep7.Data;

//var builder = WebApplication.CreateBuilder(args);

//// Voeg services toe
//builder.Services.AddControllers();
//builder.Services.AddRouting();
//builder.Services.AddAuthorization();

//// Voeg Swagger services toe
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "TreeMarket API",
//        Version = "v1"
//    });
//});

//// Voeg EF Core databasecontext toe (SQL Server Express)
//builder.Services.AddDbContext<BloggingContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalExpress")));

//var app = builder.Build();

//// Zorg dat database en tabellen bestaan
////using (var scope = app.Services.CreateScope())
////{
////    var db = scope.ServiceProvider.GetRequiredService<BloggingContext>();
////    db.Database.EnsureCreated();
////}

//// Configureer HTTP pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TreeMarket API v1");
//    });
//}

//app.UseHttpsRedirection();
//app.UseRouting();
//app.UseAuthorization();

//// Simpele weatherforecast endpoint
//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

////app.MapGet("/weatherforecast", () =>
////{
////    var forecast = Enumerable.Range(1, 5).Select(index =>
////        new WeatherForecast
////        {
////            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
////            TemperatureC = Random.Shared.Next(-20, 55),
////            Summary = summaries[Random.Shared.Next(summaries.Length)]
////        })
////        .ToArray();
////    return forecast;
////})
////.WithName("GetWeatherForecast");

//// Controllers endpoints
//app.MapControllers();

//app.Run();
