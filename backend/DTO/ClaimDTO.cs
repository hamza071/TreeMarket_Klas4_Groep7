using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class ClaimDto
    {
        [Required]
        public decimal prijs {get; set;}
        //public string klantId { get; set; }
        public int veilingId { get; set; }
    }
}
