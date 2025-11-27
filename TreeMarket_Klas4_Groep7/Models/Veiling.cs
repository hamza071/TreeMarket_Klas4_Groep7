using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMarket_Klas4_Groep7.Models
{
    public class Veiling
    {
        [Key]
        public int VeilingID { get; set; }

        public bool? Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StartPrijs { get; set; }

        [Column(TypeName = "decimal(18,2)")]

        //public decimal EindPrijs { get; set; }

        //[Column(TypeName = "decimal(18,2)")]
        public decimal HuidigePrijs { get; set; }

        public int PrijsStap { get; set; }

        public int PrijsStrategie { get; set; }

        public int TimerInSeconden { get; set; } // hoe lang de veiling duurt


        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [ForeignKey(nameof(Veilingsmeester))]
        public int VeilingsmeesterID { get; set; }
        public Veilingsmeester Veilingsmeester { get; set; }
    }
}