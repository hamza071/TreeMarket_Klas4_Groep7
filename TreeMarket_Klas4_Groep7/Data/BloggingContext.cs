//using Microsoft.EntityFrameworkCore;

//namespace TreeMarket_Klas4_Groep7.Data
//{
//    public class BloggingContext : DbContext
//    {
//        public BloggingContext(DbContextOptions<BloggingContext> options)
//            : base(options) { }

//        public DbSet<Gebruiker> Gebruikers { get; set; }
//        public DbSet<Klant> Klanten { get; set; }
//        public DbSet<Leverancier> Leveranciers { get; set; }
//        public DbSet<Veilingsmeester> Veilingsmeesters { get; set; }
//        public DbSet<Product> Producten { get; set; }
//        public DbSet<Veiling> Veilingen { get; set; }
//        public DbSet<Claim> Claims { get; set; }
//        public DbSet<Dashboard> Dashboards { get; set; }

//        protected override void OnModelCreating(ModelBuilder b)
//        {
//            // --- 1↔1 Gebruiker <-> Klant ---
//            b.Entity<Klant>().HasKey(k => k.KlantID);
//            b.Entity<Klant>()
//                .HasOne(k => k.Gebruiker)
//                .WithOne(g => g.Klant)
//                .HasForeignKey<Klant>(k => k.KlantID)
//                .OnDelete(DeleteBehavior.Cascade);

//            // --- 1↔1 Gebruiker <-> Leverancier ---
//            b.Entity<Leverancier>().HasKey(l => l.LeverancierID);
//            b.Entity<Leverancier>()
//                .HasOne(l => l.Gebruiker)
//                .WithOne(g => g.Leverancier)
//                .HasForeignKey<Leverancier>(l => l.LeverancierID)
//                .OnDelete(DeleteBehavior.Cascade);

//            // --- 1↔1 Gebruiker <-> Veilingsmeester ---
//            b.Entity<Veilingsmeester>().HasKey(v => v.VeilingsmeesterID);
//            b.Entity<Veilingsmeester>()
//                .HasOne(v => v.Gebruiker)
//                .WithOne(g => g.Veilingsmeester)
//                .HasForeignKey<Veilingsmeester>(v => v.VeilingsmeesterID)
//                .OnDelete(DeleteBehavior.Cascade);
//        }
//    }
//}



using Microsoft.EntityFrameworkCore;

namespace TreeMarket_Klas4_Groep7.Data
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options)
        { }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}