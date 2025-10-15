//using System;
//using System.Collections.Generic;

//namespace TreeMarket_Klas4_Groep7.Data
//{
//    public class Gebruiker
//    {
//        public int ID { get; set; }
//        public string Naam { get; set; }
//        public string Email { get; set; }
//        public string Telefoonnummer { get; set; }
//        public string Wachtwoord { get; set; }
//        public string Rol { get; set; }

//        public Klant Klant { get; set; }
//        public Leverancier Leverancier { get; set; }
//        public Veilingsmeester Veilingsmeester { get; set; }
//    }

//    public class Klant
//    {
//        public int KlantID { get; set; }             // PK = FK naar Gebruiker
//        public Gebruiker Gebruiker { get; set; }
//        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
//    }

//    public class Leverancier
//    {
//        public int LeverancierID { get; set; }       // PK = FK naar Gebruiker
//        public string Bedrijf { get; set; }
//        public string KvKnummer { get; set; }
//        public string IBANnummer { get; set; }
//        public Gebruiker Gebruiker { get; set; }
//        public ICollection<Product> Producten { get; set; } = new List<Product>();
//    }

//    public class Veilingsmeester
//    {
//        public int VeilingsmeesterID { get; set; }   // PK = FK naar Gebruiker
//        public DateTime? PlantDatum { get; set; }
//        public Gebruiker Gebruiker { get; set; }
//        public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();
//    }

//    public class Product
//    {
//        public int ID { get; set; }
//        public string Foto { get; set; }
//        public string Artikelenkenmerken { get; set; }
//        public int Hoeveelheid { get; set; }
//        public decimal MinimumPrijs { get; set; }
//        public DateTime? Dagdatum { get; set; }

//        public int LeverancierID { get; set; }
//        public Leverancier Leverancier { get; set; }

//        public int? DashboardID { get; set; }
//        public Dashboard Dashboard { get; set; }

//        public Veiling Veiling { get; set; }          // 1-op-1
//    }

//    public class Veiling
//    {
//        public int VeilingID { get; set; }
//        public bool Status { get; set; }
//        public decimal StartPrijs { get; set; }
//        public decimal? EindPrijs { get; set; }
//        public decimal? HuidigPrijs { get; set; }
//        public string PrijsStrategie { get; set; }
//        public DateTime StartTijd { get; set; }
//        public DateTime? EindTijd { get; set; }

//        public int ProductID { get; set; }
//        public Product Product { get; set; }

//        public int VeilingsmeesterID { get; set; }
//        public Veilingsmeester Veilingsmeester { get; set; }

//        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
//    }

//    public class Claim
//    {
//        public int ID { get; set; }
//        public decimal Prijs { get; set; }

//        public int KlantID { get; set; }
//        public Klant Klant { get; set; }

//        public int VeilingID { get; set; }
//        public Veiling Veiling { get; set; }
//    }

//    public class Dashboard
//    {
//        public int ID { get; set; }
//        public ICollection<Product> Producten { get; set; } = new List<Product>();
//    }
//}




namespace TreeMarket_Klas4_Groep7.Data
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }
        public List<Post> Posts { get; } = new();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
}