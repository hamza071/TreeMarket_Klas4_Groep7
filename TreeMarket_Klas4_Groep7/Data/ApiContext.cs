using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Models;
using TreeMarket_Klas4_Groep7.ToDo;

namespace TreeMarket_Klas4_Groep7.Data
{
    //Moet nog verbonden worden met de database...
    public class ApiContext : DbContext
    {
        //Roept van bestaande klasses
        public DbSet<Product> Product { get; set; }
        public DbSet<Gebruiker> Gebruiker { get; set; }
        public DbSet<Veiling> Veiling { get; set; }
        public DbSet<Dashboard> Dashboard { get; set; }
        public DbSet<Claim> Claim { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }

        //Deze methode zorgt ervoor om de data in de database op te slaan.
        protected override void OnConfiguring(DbContextOptionsBuilder b) 
            => b.UseSqlServer("Data Source=localhost\\SQLEXPRESS;" +
                                            "Initial Catalog=instnwnd;" +
                                            "Integrated Security = True;" +
                                            "Encrypt = True;" +
                                            "TrustServerCertificate=True;");
        // b.UseSqlServer("Connection string") //Je kan de keuze van de database ook in de startup van de app plaatsen


        //Deze methode is voor de child klasse van de Gebruiker klasse.
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

        }

    }
}