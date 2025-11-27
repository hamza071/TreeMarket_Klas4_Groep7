using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models
{
    public class Veiling
    {
        [Key]
        public int VeilingID { get; set; }

        [Required]
        public decimal StartPrijs { get; set; }

        [Required]
        public int PrijsStap { get; set; }

        [Required]
        public int SluitingstijdInSeconden { get; set; }

        // Extra velden voor frontend
        public string Code { get; set; }
        public string Naam { get; set; }
        public string Beschrijving { get; set; }
        public int Lots { get; set; }
        public string Status { get; set; } = "pending";
        public string Image { get; set; }

        // Relaties
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int VeilingsmeesterID { get; set; }
        public Veilingsmeester Veilingsmeester { get; set; }
    }
}