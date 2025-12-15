using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMarket_Klas4_Groep7.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public string Foto { get; set; }

    [Required]
    public string ProductNaam { get; set; }

    [Required]
    public int Hoeveelheid { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MinimumPrijs { get; set; }

    [Required]
    public DateTime Dagdatum { get; set; }

    [ForeignKey(nameof(Leverancier))]
    public string LeverancierID { get; set; }
    public Leverancier? Leverancier { get; set; }

    [Required]
    public string Varieteit { get; set; }

    [Required]
    public string Omschrijving { get; set; }

    public ICollection<Veiling> Veilingen { get; set; } = new List<Veiling>();
}