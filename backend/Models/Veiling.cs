using backend.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.DTO;

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
    public decimal MinPrijs { get; set; } = 0; // default = 0

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PrijsStap { get; set; }

    public int PrijsStrategie { get; set; }

    [Required]
    public int TimerInSeconden { get; set; }

    [Required]
    public DateTime StartTimestamp { get; set; }

    

    [Required]
    public int ProductID { get; set; }
    public Product Product { get; set; }

    [Required]
    public string VeilingsmeesterID { get; set; }
    public Gebruiker Veilingsmeester { get; set; }
}