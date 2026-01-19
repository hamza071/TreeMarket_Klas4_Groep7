using backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using backend.Data;
using backend.Models;
using backend.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Dit wordt alleen gebruikt binnen de functie CreateProduct van Service en Controller😂
// Reden: omdat in de Nederlandse een punt geen komma is, maar binnen EN-US is een , en . beide komma's
//Bijvoorbeeld voor NL 10,50 = 10.50, maar niet als het 10.50 = 105 is. Maar voor en-US 10.50 = 10.50 en ook met 10,50 = 10.50
var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

//
// 1. Database configuratie
// Zorg dat je connection string in appsettings.json klopt!
var connectionString = builder.Configuration.GetConnectionString("LocalExpress")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure())
);

// ==========================
// 2) IDENTITY + AUTH
// ==========================
builder.Services.AddIdentity<Gebruiker, IdentityRole>()
    .AddEntityFrameworkStores<ApiContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddTransient<IEmailSender<Gebruiker>, DummyEmailSender>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.BearerScheme;
    options.DefaultChallengeScheme = IdentityConstants.BearerScheme;
})
.AddBearerToken(IdentityConstants.BearerScheme, options =>
{
    options.BearerTokenExpiration = TimeSpan.FromMinutes(60.0);
});

builder.Services.AddAuthorization();

// ==========================
// 3) DI: SERVICES
// ==========================
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGebruikerService, GebruikerService>();
builder.Services.AddScoped<IVeilingService, VeilingService>();
builder.Services.AddScoped<ILeverancierService, LeverancierService>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Controllers + Swagger
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

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

// CORS
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

// ==========================
// 4) SEEDING: ROLLEN + ADMIN + TEST USER
// ==========================
using (var scope = app.Services.CreateScope())
{
    //// 1. HAAL EERST DE DATABASE CONTEXT OP
    //var context = scope.ServiceProvider.GetRequiredService<ApiContext>();

    //// 2. VOER DE MIGRATIES UIT (MAAK TABELLEN AAN IN AZURE)
    //// Dit commando zorgt dat de database tabellen worden aangemaakt als ze nog niet bestaan.
    //context.Database.Migrate();

    //// ---------------------------------------------------------

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Gebruiker>>();

    // Rollen aanmaken
    string[] roles = { "Admin", "Klant", "Leverancier", "Veilingsmeester" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // -------- Admin seeden (robust) --------
    var adminEmail = "admin@treemarket.nl";
    var adminPassword = "AppelKruimel1234!";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    // 1) Admin maken als hij niet bestaat
    if (adminUser == null)
    {
        adminUser = new Gebruiker
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Naam = "Super Admin"
        };

        // Identity hasht het wachtwoord automatisch
        var result = await userManager.CreateAsync(adminUser, "AppelKruimel1234!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    // -------- Test Veilingsmeester --------
    var testEmail = "test@treemarket.nl";
    var testPassword = "Veiling123!";
    var testUser = await userManager.FindByEmailAsync(testEmail);

    if (testUser == null)
    {
        testUser = new Gebruiker
        {
            UserName = testEmail,
            Email = testEmail,
            EmailConfirmed = true,
            Naam = "Veilingsmeester Test"
        };

        var createResult = await userManager.CreateAsync(testUser, testPassword);
        if (createResult.Succeeded)
        {
            Console.WriteLine("Test gebruiker aangemaakt: " + testEmail);
        }
        else
        {
            Console.WriteLine("Test gebruiker aanmaken mislukt:");
            foreach (var err in createResult.Errors)
            {
                Console.WriteLine($"- {err.Code}: {err.Description}");
            }
        }
    }

    if (testUser != null && !await userManager.IsInRoleAsync(testUser, "Veilingsmeester"))
    {
        await userManager.AddToRoleAsync(testUser, "Veilingsmeester");
        Console.WriteLine("Rol Veilingsmeester toegevoegd aan: " + testEmail);
    }
}

// ==========================
// 5) MIDDLEWARE PIPELINE
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Identity endpoints: /login, /register, etc.
app.MapIdentityApi<Gebruiker>();

app.MapControllers();

app.Run();
