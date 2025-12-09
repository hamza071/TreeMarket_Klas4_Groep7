//Deze code wordt gebruikt om tokens te generen voor de login. 
//Dit is ook in de appsettings.json gezet :)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Interfaces;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;


var builder = WebApplication.CreateBuilder(args);

// JWT-config ophalen
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("Key"); // Let op: Key, niet SecretKey


// EF Core
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LocalExpress")));

// REST API Controllers
//Voor nu controllers zonder views. Later kunnen we met views gebruiken zodien dat nodig is.
builder.Services.AddControllers();

//EF Core Test of de controller wel de database pakt binnen de appsettings.json
Console.WriteLine("==== Active Connection ====");
Console.WriteLine(builder.Configuration.GetConnectionString("LocalExpress"));

// ✅ Voeg controllers + views toe (voor MVC + Razor)
builder.Services.AddControllersWithViews();

// =======================
// Swagger (voor API testing / documentation)
// =======================
builder.Services.AddEndpointsApiExplorer();
// =======================
// Swagger met JWT Support
// =======================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TreeMarket API", Version = "v1" });

    // Voeg de "Authorize" knop toe
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
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

// ============== De Controller klasses maakt gebruik van een interface :)==============
// ProductService kan nu via constructor in controllers worden gebruikt
builder.Services.AddScoped<IProductController, ProductService>();
builder.Services.AddScoped<IGebruikerController, GebruikerService>();
builder.Services.AddScoped<IVeilingController, VeilingService>();
builder.Services.AddScoped<ILeverancierController, LeverancierService>();
builder.Services.AddScoped<IClaimController, ClaimService>();
builder.Services.AddScoped<IDashboardController, DashboardService>();


// De PasswordHasher zorgt ervoor dat het wachtwoord gehashed is.
//Dit wordt alleen gebruik voor het tabel Gebruiker en zijn kinderen (sub klasses).
builder.Services.AddScoped<PasswordHasher<Gebruiker>>();



// ===============
// CORS = Cross-Origin Resource Sharing
// Het regelt en webpagina die geladen wordt vanaf een domein zoals http://localhost:55125
// CORS gebruikt ook de Grand Cross om met andere domeinen te mogen communiceren via een server.
// ===========

// Tijdelijk voor testen
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactDev", policy =>
//    {
//        //De nummer van de localhost staat op basis van hoe wij npm run dev starten.
//        policy.WithOrigins("http://localhost:55125")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//=====De builder voor de JWT token.=====
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// De jwt wordt geautoriseerd.
builder.Services.AddAuthorization();


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
//tijdelijk
app.UseCors("AllowAll"); //app.UseCors("AllowReactDev");
app.UseAuthentication();
//Wordt waarschijnlijk gebruikt voor rechten en rollen
app.UseAuthorization();

// Map alle controllers
app.MapControllers();

////De app runt via een localhost
//app.Run();

// =======================
// Run the app
// =======================
app.Run();