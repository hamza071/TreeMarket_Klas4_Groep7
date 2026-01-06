using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Data
{
    public class ApiContext : IdentityDbContext<Gebruiker>
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<Dashboard> Dashboard { get; set; }
        public DbSet<Claim> Claim { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Veiling> Veiling { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BELANGRIJK: base call eerst
            base.OnModelCreating(modelBuilder);

            // TPH: alles in AspNetUsers
            modelBuilder.Entity<Gebruiker>()
                .HasDiscriminator<string>("GebruikerType")
                .HasValue<Klant>("Klant")
                .HasValue<Leverancier>("Leverancier")
                .HasValue<Veilingsmeester>("Veilingsmeester");

            // Relaties
            modelBuilder.Entity<Veiling>()
                .HasOne(v => v.Product)
                .WithMany(p => p.Veilingen)
                .HasForeignKey(v => v.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Leverancier)
                .WithMany(l => l.Producten)
                .HasForeignKey(p => p.LeverancierID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Veiling>()
                .HasOne(v => v.Veilingsmeester)
                .WithMany()
                .HasForeignKey(v => v.VeilingsmeesterID)
                .OnDelete(DeleteBehavior.Restrict);

            // Cascade delete voorkomen voor subtypes
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                  .SelectMany(t => t.GetForeignKeys())
                  .Where(fk => fk.PrincipalEntityType.ClrType.IsSubclassOf(typeof(Gebruiker))))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Decimals
            modelBuilder.Entity<Claim>()
                .Property(c => c.Prijs)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>()
                .Property(p => p.MinimumPrijs)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Veiling>()
                .Property(v => v.StartPrijs)
                .HasColumnType("decimal(18,2)");
        }
    }
}
