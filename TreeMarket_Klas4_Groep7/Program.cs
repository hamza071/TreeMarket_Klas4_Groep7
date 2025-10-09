using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Data.SqlClient;
using TreeMarket_Klas4_Groep7;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddRouting();
builder.Services.AddAuthorization();

// EF Core: SQL Server via connection string "Sql"
builder.Services.AddDbContext<BloggingContext>(o =>
    o.UseSqlServer(builder.Configuration.GetConnectionString("Sql")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Optioneel: DB-connectivity test
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