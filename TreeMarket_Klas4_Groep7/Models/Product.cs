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
        //De decimal geeft hoeveel decimal het minimaal mag
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumPrijs { get; set; }
        [Required]
        public DateTime Dagdatum { get; set; }

        //Een op meer relatie
        [ForeignKey(nameof(Leverancier))]
        public int LeverancierID { get; set; }
        public Leverancier Leverancier { get; set; }

        //Voor de LINQ om de veilingen toe te voegen
        public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();

    }
}