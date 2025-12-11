using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Veiling
    {
        [Key]
        public int VeilingID { get; set; }

        public bool? Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StartPrijs { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HuidigePrijs { get; set; }

        // AANPASSING 1: Decimal gemaakt voor percentages (of bedragen met centen)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrijsStap { get; set; } // Was int

        public int PrijsStrategie { get; set; }

        public int TimerInSeconden { get; set; } 

        // --- RELATIES ---

        // Product heeft zijn eigen int ID, dus dit blijft int
        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        // AANPASSING 2: Veilingsmeester is een IdentityUser, dus STRING ID!
        [ForeignKey(nameof(Veilingsmeester))]
        public string VeilingsmeesterID { get; set; } // Was int
        public Veilingsmeester Veilingsmeester { get; set; }

    }
}