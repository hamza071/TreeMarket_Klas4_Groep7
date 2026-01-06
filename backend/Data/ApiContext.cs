using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Security.Cryptography;
using backend.Models;
using backend.Models.DTO;

namespace backend.Data
{
    // AANGEPAST: Erft nu van IdentityDbContext<Gebruiker> in plaats van DbContext
    public class ApiContext : IdentityDbContext<Gebruiker>
    {
        public DbSet<Product> Product { get; set; }
        // DbSet<Gebruiker> hoeft eigenlijk niet meer (zit in IdentityDbContext als 'Users'), 
        // maar je mag hem laten staan als je oude code 'context.Gebruiker' gebruikt.
        public DbSet<Gebruiker> Gebruiker { get; set; }
        public DbSet<Dashboard> Dashboard { get; set; }
        public DbSet<Claim> Claim { get; set; }
        public DbSet<Leverancier> Leverancier { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Veiling> Veiling { get; set; }

        // Vergeet de andere sub-types niet als je die apart wilt kunnen aanroepen!


        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BELANGRIJK: Deze 'base' call moet als EERSTE staan.
            // Dit zorgt ervoor dat Identity alle tabellen (AspNetRoles, etc.) aanmaakt.
            base.OnModelCreating(modelBuilder);

            // === JOUW TABEL NAMEN ===
            // Hiermee overschrijf je de standaard Identity namen (zoals AspNetUsers)
            // naar je eigen namen. Dit is goed!

            modelBuilder.Entity<Gebruiker>()
            .HasDiscriminator<string>("GebruikerType")
            .HasValue<Gebruiker>("Gebruiker")
            .HasValue<Klant>("Klant")
            .HasValue<Leverancier>("Leverancier")
            .HasValue<Veilingsmeester>("Veilingsmeester");


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
                
            // (Je had HuidigePrijs uitgecommentarieerd, die laat ik hier ook weg)
        }
    }
}