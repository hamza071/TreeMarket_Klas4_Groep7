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
        public int VeilingID { get; set; } // Blijft int (want Veiling.VeilingID is int)

        [ForeignKey(nameof(VeilingID))]
        public Veiling Veiling { get; set; }

        // === TOEVOEGING: WIE HEEFT GEBODEN? ===
        // Dit moest erbij. En omdat Klant nu IdentityUser is,
        // moet dit type 'string' zijn!
        [Required]
        public string KlantId { get; set; } 

        [ForeignKey(nameof(KlantId))]
        public Klant Klant { get; set; }
        // ======================================

        [Column(TypeName = "decimal(18,2)")]
        public decimal Bedrag { get; set; }

        public DateTime Tijdstip { get; set; } = DateTime.UtcNow;
    }
}