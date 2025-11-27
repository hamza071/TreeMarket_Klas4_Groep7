using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.Models.DTO;

namespace TreeMarket_Klas4_Groep7.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Product> Product { get; set; }
        public DbSet<Gebruiker> Gebruiker { get; set; }
        public DbSet<Veiling> Veiling { get; set; }
        public DbSet<Dashboard> Dashboard { get; set; }
        public DbSet<Claim> Claim { get; set; }
        public DbSet<Leverancier> Leverancier { get; set; }
        public DbSet<Bid> Bids { get; set; }


        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // === HIER ZAT HET PROBLEEM ===
            // Gebruik ToTable("Naam") om aan te geven dat het aparte tabellen zijn in SQL
            modelBuilder.Entity<Gebruiker>().ToTable("Gebruiker");
            modelBuilder.Entity<Klant>().ToTable("Klant");
            modelBuilder.Entity<Leverancier>().ToTable("Leverancier");
            modelBuilder.Entity<Veilingsmeester>().ToTable("Veilingsmeester");
            // ==============================

            // Relaties configureren
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

            // Cascade delete voorkomen
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                  .SelectMany(t => t.GetForeignKeys())
                  .Where(fk => fk.PrincipalEntityType.ClrType.IsSubclassOf(typeof(Gebruiker))))
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // Decimals instellen
            modelBuilder.Entity<Claim>()
                .Property(c => c.Prijs)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Product>()
                .Property(p => p.MinimumPrijs)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Veiling>()
                .Property(v => v.StartPrijs)
                .HasColumnType("decimal(18,2)");
            //modelBuilder.Entity<Veiling>()
            //    .Property(v => v.EindPrijs)
            //    .HasColumnType("decimal(18,2)");

            // LET OP: Ik heb HuidigePrijs hier weggehaald, want die had je ook uit je SQL script gehaald!
            // Als je die laat staan, crasht hij straks weer.
        }
    }
}