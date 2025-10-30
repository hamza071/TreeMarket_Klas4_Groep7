using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Views;

namespace TreeMarket_Klas4_Groep7.Data
{
    //Moet nog verbonden worden met de database...
    public class ApiContext : DbContext
    {
        //Roept van bestaande klasses
        public DbSet<Product> Product { get; set; }
        public DbSet<Gebruiker> Gebruiker { get; set; }
        //klant, leverancier en veilingsmeester is de subklasse van de gebruiker
        public DbSet<Klant> Klant { get; set; }
        public DbSet<Veilingsmeester> Veilingsmeester { get; set; }
        public DbSet<Leverancier> Leverancier { get; set; }
        public DbSet<Veiling> Veiling { get; set; }
        public DbSet<Dashboard> Dashboard { get; set; }
        public DbSet<Claim> Claim { get; set; }


        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }

    }
}