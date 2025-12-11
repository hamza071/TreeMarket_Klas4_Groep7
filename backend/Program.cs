using backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Services;
var builder = WebApplication.CreateBuilder(args);
//
// 1. Database configuratie
// Zorg dat je connection string in appsettings.json klopt!
var connectionString = builder.Configuration.GetConnectionString("LocalExpress") 
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));

// ============================================================
// 2. IDENTITY CONFIGURATIE (Volgens de Slides)
// ============================================================

// Slide 3: AddIdentity vervangt je handmatige configuratie
builder.Services.AddIdentity<Gebruiker, IdentityRole>()
    .AddEntityFrameworkStores<ApiContext>()
    .AddDefaultTokenProviders();

// Slide 5: Extra services toevoegen
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddTransient<IEmailSender<Gebruiker>, DummyEmailSender>();

// Slide 11: Authenticatie instellen op Bearer Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
})
.AddBearerToken(IdentityConstants.BearerScheme, options =>
{
    options.BearerTokenExpiration = TimeSpan.FromMinutes(60.0);
});

// Authorization aanzetten
builder.Services.AddAuthorization();

// ============================================================

// Services toevoegen
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

// Swagger instellen (Slide 11)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TreeMarket API", Version = "v1" });
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Vul hier je token in (alleen de code)",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

// ============== De Controller klasses maakt gebruik van een interface :)==============
// Je eigen services
builder.Services.AddScoped<IProductController, ProductService>();
builder.Services.AddScoped<IGebruikerController, GebruikerService>();
//builder.Services.AddScoped<IVeilingController, VeilingService>();
builder.Services.AddScoped<ILeverancierController, LeverancierService>();
builder.Services.AddScoped<IClaimController, ClaimService>();
builder.Services.AddScoped<IDashboardController, DashboardService>();

// CORS beleid
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============================================================
// 3. SEEDING: Admin en Rollen aanmaken bij opstarten
// (Slide 6 & 8)
// ============================================================
using (var scope = app.Services.CreateScope())
{
    // 1. HAAL EERST DE DATABASE CONTEXT OP
    var context = scope.ServiceProvider.GetRequiredService<ApiContext>();
    
    // 2. VOER DE MIGRATIES UIT (MAAK TABELLEN AAN IN AZURE)
    // Dit commando zorgt dat de database tabellen worden aangemaakt als ze nog niet bestaan.
    context.Database.Migrate();

    // ---------------------------------------------------------

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Gebruiker>>();

    // Rollen aanmaken
    string[] roles = ["Admin", "Klant", "Leverancier", "Veilingsmeester"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Admin aanmaken
    var adminEmail = "admin@treemarket.nl";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var user = new Gebruiker
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Naam = "Super Admin"
        };
        
        // Identity hasht het wachtwoord automatisch
        var result = await userManager.CreateAsync(user, "AppelKruimel1234!");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// ZET DEZE AAN: Dit maakt de /login en /register endpoints
app.MapIdentityApi<Gebruiker>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();//