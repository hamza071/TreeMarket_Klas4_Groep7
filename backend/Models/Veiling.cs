using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TreeMarket_Klas4_Groep7.Models;

public class Veiling
{
    [Key]
    public int VeilingID { get; set; }

        /// <summary>
        /// Geeft aan of de veiling actief is. True = actief, False = gesloten.
        /// </summary>
        public bool Status { get; set; } = true; // standaard actief

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "StartPrijs moet groter dan 0 zijn.")]
        public decimal StartPrijs { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "HuidigePrijs moet groter dan 0 zijn.")]
        public decimal HuidigePrijs { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "PrijsStap moet groter dan 0 zijn.")]
        public decimal PrijsStap { get; set; } // Bod-stap per bieding

        /// <summary>
        /// Type of strategie voor prijsstijging (bijv. lineair of exponentieel)
        /// </summary>
        public int PrijsStrategie { get; set; } = 0; // standaard strategie = 0

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Timer moet minimaal 1 seconde zijn.")]
        public int TimerInSeconden { get; set; }

        // ================= RELATIES =================

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Required]
        [ForeignKey(nameof(Veilingsmeester))]
        public string VeilingsmeesterID { get; set; } // IdentityUser ID
        public Veilingsmeester Veilingsmeester { get; set; }
    }
}