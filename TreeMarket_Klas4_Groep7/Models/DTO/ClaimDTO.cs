using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class ClaimDto
    {
        [Required]
        public decimal prijs {get; set;}
        public int klantId { get; set; }
        public int veilingId { get; set; }
    }
}
