using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalExpress")));

// REST API Controllers
//Voor nu controllers zonder views. Later kunnen we met views gebruiken zodien dat nodig is.
builder.Services.AddControllers();

//EF Core Test of de controller wel de database pakt binnen de appsettings.json
Console.WriteLine("==== Active Connection ====");
Console.WriteLine(builder.Configuration.GetConnectionString("LocalExpress"));


//builder.Services.AddControllersWithViews();


// ✅ Voeg controllers + views toe (voor MVC + Razor)
builder.Services.AddControllersWithViews();

////Deze regel doet nu niks
//builder.Services.AddRouting();

// =======================
// Swagger (voor API testing / documentation)
// =======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "TreeMarket API",
//        Version = "v1"
//    });
//});       

// =======================
// Dependency Injection
// =======================

// ProductService kan nu via constructor in controllers worden gebruikt
builder.Services.AddScoped<ProductService>();

// De PasswordHasher zorgt ervoor dat het wachtwoord gehashed is.
//Dit wordt alleen gebruik voor het tabel Gebruiker en zijn kinderen (sub klasses).
builder.Services.AddScoped<PasswordHasher<Gebruiker>>();



// ===============
// CORS = Cross-Origin Resource Sharing
// Het regelt en webpagina die geladen wordt vanaf een domein zoals http://localhost:55125
// CORS gebruikt ook de Grand Cross om met andere domeinen te mogen communiceren via een server.
// ===========

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
    {
        //De nummer van de localhost staat op basis van hoe wij npm run dev starten.
        policy.WithOrigins("http://localhost:55125")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// =======================
// Build the app
// =======================
var app = builder.Build();

// =======================
// Configure middleware
// =======================

if (app.Environment.IsDevelopment())
{
    // Swagger UI alleen in development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Routing en Authorization
app.UseRouting();

//CORS wordt opgeroepen.
//Dit zorgt ervoor dat de localhost binnen
app.UseCors("AllowReactDev");
app.UseAuthorization();

// Map alle controllers
app.MapControllers();

////De app runt via een localhost
//app.Run();

// =======================
// Run the app
// =======================
app.Run();