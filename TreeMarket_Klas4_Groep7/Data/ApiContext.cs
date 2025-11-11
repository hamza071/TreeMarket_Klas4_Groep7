using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.ToDo;

namespace TreeMarket_Klas4_Groep7.Data
{
    //Database wordt opgeroepen binnen de Program.cs
    public class ApiContext : DbContext
    {
        //Roept van bestaande klasses
        public DbSet<Product> Producten { get; set; }
        public DbSet<Gebruiker> Gebruikers { get; set; }
        public DbSet<Veiling> Veilingen { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<Claim> Claims { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {
        }

        
        //Deze methode is voor de child klasse van de Gebruiker klasse.
        //Dit wordt ook gebruikt voor unieke annotaties.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //De base.OnModelCreating zorgt ervoor dat de data niet overgeschreven worden wat het ook veiliger maakt.
            base.OnModelCreating(modelBuilder);

            //Modelbuilder configureert de structuur van de database.
            //klant, leverancier en veilingsmeester is de subklasse van de gebruiker
            //De modelbuilder wordt gebruikt als andere tabel een FK zit van andere tabellen.
            modelBuilder.Entity<Klant>().HasBaseType<Gebruiker>();
            modelBuilder.Entity<Leverancier>().HasBaseType<Gebruiker>();
            modelBuilder.Entity<Veilingsmeester>().HasBaseType<Gebruiker>();
            //modelBuilder.Entity<Product>()
            //.HasOne(p => p.Leverancier)
            //.WithMany()
            //.HasForeignKey(p => p.LeverancierId);

            //Dit zijn van de datatype decimaal waar het ook bij de Model zelf toegevoegd is.
            modelBuilder.Entity<Claim>()
                .Property(c => c.Prijs)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.MinimumPrijs)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Veiling>()
                .Property(v => v.StartPrijs)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Veiling>()
                .Property(v => v.EindPrijs)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Veiling>()
                .Property(v => v.HuidigePrijs)
                .HasColumnType("decimal(18,2)");
        }
    }
}