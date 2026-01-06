using backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;
using System.Security.Claims;

using backend.Data;
using backend.Interfaces;
using backend.Models;
using backend.Services;
var builder = WebApplication.CreateBuilder(args);

// 1. Database configuratie
// ==========================
var connectionString = builder.Configuration.GetConnectionString("LocalExpress")
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));

// ==========================
// 2. Identity configuratie
// ==========================
builder.Services.AddIdentity<Gebruiker, IdentityRole>()
    .AddEntityFrameworkStores<ApiContext>()
    .AddDefaultTokenProviders();

// Extra services
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddTransient<IEmailSender<Gebruiker>, DummyEmailSender>();

// Authenticatie instellen op Bearer Token
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
// 3. Controllers en services
// ==========================
builder.Services.AddScoped<IProductController, ProductService>();
builder.Services.AddScoped<IGebruikerController, GebruikerService>();
builder.Services.AddScoped<IVeilingController, VeilingService>();
builder.Services.AddScoped<ILeverancierController, LeverancierService>();
builder.Services.AddScoped<IClaimController, ClaimService>();
builder.Services.AddScoped<IDashboardController, DashboardService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==========================
// 4. Swagger configuratie
// ==========================
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
builder.Services.AddScoped<IVeilingController, VeilingService>();
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

// ==========================
// 6. Database seeding
// ==========================
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

    string[] roles = { "Admin", "Klant", "Leverancier", "Veilingsmeester" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

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

        var result = await userManager.CreateAsync(user, "AppelKruimel1234!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    // ==========================
    // Toevoegen test Veilingsmeester
    // ==========================
    var testEmail = "test@treemarket.nl";
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

        var createResult = await userManager.CreateAsync(testUser, "Veiling123!");
        if (createResult.Succeeded)
        {
            Console.WriteLine("Test gebruiker aangemaakt: " + testEmail);
        }
    }

    if (!await userManager.IsInRoleAsync(testUser, "Veilingsmeester"))
    {
        await userManager.AddToRoleAsync(testUser, "Veilingsmeester");
        Console.WriteLine("Rol Veilingsmeester toegevoegd aan: " + testEmail);
    }
}

// ==========================
// 7. Middleware pipeline
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

// Identity endpoints (login, register, etc.)
app.MapIdentityApi<Gebruiker>();

// Controllers
app.MapControllers();

app.Run();//