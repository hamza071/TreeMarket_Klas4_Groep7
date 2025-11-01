using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TreeMarket_Klas4_Groep7.Views
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
        public decimal MinimumPrijs { get; set; }
        [Required]
        public DateTime Dagdatum { get; set; }

        //Een op meer relatie
        [ForeignKey(nameof(Leverancier))]
        public decimal LeverancierID { get; set; }
        public Leverancier Leverancier { get; set; }
    }
}