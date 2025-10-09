using Microsoft.EntityFrameworkCore;

namespace TreeMarket_Klas4_Groep7;

public class AppDb : DbContext
{
    public AppDb(DbContextOptions<AppDb> options) : base(options) { }

    // Example table so it compiles and can run
    public DbSet<Demo> Demos => Set<Demo>();
}

public class Demo
{
    public int Id { get; set; }
    public required string Note { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}