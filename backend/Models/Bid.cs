using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Bid
    {
        [Key]
        public int BidID { get; set; }

        [Required]
        public int VeilingID { get; set; } 

        [ForeignKey(nameof(VeilingID))]
        public Veiling Veiling { get; set; }

        
        
        [Required]
        public string KlantId { get; set; } 

        [ForeignKey(nameof(KlantId))]
        public Klant Klant { get; set; }
        

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        public DateTime Tijdstip { get; set; } = DateTime.UtcNow;
    }
}