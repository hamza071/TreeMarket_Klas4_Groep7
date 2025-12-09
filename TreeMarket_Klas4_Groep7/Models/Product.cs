using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMarket_Klas4_Groep7.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Foto { get; set; }

        [Required]
        public string Artikelkenmerken { get; set; }

        [Required]
        public int Hoeveelheid { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumPrijs { get; set; }

        [Required]
        public DateTime Dagdatum { get; set; }

        // --- AANPASSING: Leverancier is een IdentityUser, dus STRING ID! ---
        [ForeignKey(nameof(Leverancier))]
        public string LeverancierID { get; set; } // <--- Was int, nu string!
        
        public Leverancier? Leverancier { get; set; }

        public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();
    }
}