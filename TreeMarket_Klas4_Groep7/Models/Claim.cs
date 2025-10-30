using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TreeMarket_Klas4_Groep7.Views
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        //Het staat in de RIM als INT, maar de rest is decimal
        [Required]
        public decimal Prijs { get; set; }
        //Foreign key
        public int KlantId { get; set; }
        public int VeilingId { get; set; }
    }
}