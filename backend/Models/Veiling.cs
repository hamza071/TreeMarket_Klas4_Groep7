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
    public decimal StartPrijs { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal HuidigePrijs { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrijsStap { get; set; }

    public int PrijsStrategie { get; set; }

    [Required]
    public int TimerInSeconden { get; set; }

    // ---------- RELATIES ----------

    [Required]
    public int ProductID { get; set; }
    public Product Product { get; set; }

    // ✅ FK naar Veilingsmeester.Id (INT)
    [Required]
    public string VeilingsmeesterID { get; set; }
    public Veilingsmeester Veilingsmeester { get; set; }
}