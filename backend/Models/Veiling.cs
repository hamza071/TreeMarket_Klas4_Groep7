using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMarket_Klas4_Groep7.Models;

public class Veiling
{
    [Key]
    public int VeilingID { get; set; }

    public bool Status { get; set; } = true;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal StartPrijs { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal HuidigePrijs { get; set; }

    // ✅ MOET bestaan, maar NIET uit frontend komen
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal PrijsStap { get; set; }

    public int PrijsStrategie { get; set; } = 0;

    [Required]
    [Range(1, int.MaxValue)]
    public int TimerInSeconden { get; set; }

    [Required]
    [ForeignKey(nameof(Product))]
    public int ProductID { get; set; }
    public Product Product { get; set; }

    [Required]
    [ForeignKey(nameof(Veilingsmeester))]
    public string VeilingsmeesterID { get; set; }
    public Veilingsmeester Veilingsmeester { get; set; }
}