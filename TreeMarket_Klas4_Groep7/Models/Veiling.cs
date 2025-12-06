// Veiling model aanpassen
using System.ComponentModel.DataAnnotations;
using TreeMarket_Klas4_Groep7.Models;

public class Veiling
{
    [Key]
    public int VeilingID { get; set; }

    [Required]
    public decimal? StartPrijs { get; set; } // nullable voor concept veilingen

    [Required]
    public int? PrijsStap { get; set; } // nullable

    [Required]
    public int? SluitingstijdInSeconden { get; set; } // nullable

    public string Code { get; set; }
    public string Naam { get; set; }
    public string Beschrijving { get; set; }
    public int Lots { get; set; }
    public string Status { get; set; } = "pending";
    public string Image { get; set; }

    public int ProductID { get; set; }
    public Product Product { get; set; }

    public int? VeilingsmeesterID { get; set; } // nullable
    public Veilingsmeester Veilingsmeester { get; set; }
}