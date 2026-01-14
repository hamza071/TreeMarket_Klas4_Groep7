using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Claim
    {
        [Key]
        [Required]
        public int ClaimID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Prijs { get; set; }

       
        [Required]
        public int Aantal { get; set; } 

        [ForeignKey(nameof(Klant))]
        public string KlantId { get; set; } 
        public Klant klant { get; set; }

        // VeilingId blijft int, want Veiling.VeilingID is nog steeds een int
        [ForeignKey(nameof(Veiling))]
        public int VeilingId { get; set; }
        public Veiling Veiling { get; set; }
    }
}