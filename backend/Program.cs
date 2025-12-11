using backend.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TreeMarket_Klas4_Groep7.Data;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================
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

// ==========================
// 5. CORS beleid
// ==========================
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
}

// ==========================
// 7. Middleware pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Identity endpoints (login, register, etc.)
app.MapIdentityApi<Gebruiker>();

// Controllers
app.MapControllers();

app.Run();