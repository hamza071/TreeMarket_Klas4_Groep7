using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TreeMarket_Klas4_Groep7.Views;

namespace TreeMarket_Klas4_Groep7.Data
{
    public class ApiContext : DbContext
    {
        //Roept van bestaande klasses
        public DbSet<Product> Product { get; set; }
        public DbSet<Gebruiker> Gebruiker { get; set; }
        //klant, leverancier en veilingsmeester is de subklasse van de gebruiker
        public DbSet<Klant> Klant { get; set; }


        public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
        {

        }

    }
}