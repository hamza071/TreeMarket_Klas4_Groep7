using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace TreeMarket_Klas4_Groep7.Models
{
    public class Veiling
    {
        [Key]
        [Required]
        public int VeilingID { get; set; }
        //Status is met BIT. IK weet niet welke. Misschien int of bool
        public bool? Status { get; set; }

        //De decimal geeft hoeveel decimal het minimaal mag
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartPrijs { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EindPrijs { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HuidigePrijs { get; set; }

        public int PrijsStap { get; set; }

        public int PrijsStrategie { get; set; }

        public DateTime StartTijd { get; set; }

        public DateTime EindTijd { get; set; }

        //Is wel een foreignkey
        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey(nameof(Veilingsmeester))]
        public int VeilingsmeesterID { get; set; }
        public Veilingsmeester Veilingsmeester { get; set; }
    }
}