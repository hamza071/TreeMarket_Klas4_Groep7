using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class ClaimDto
    {
        [Required]
        public decimal Prijs { get; set; }  

        [Required]
        public int VeilingId { get; set; }  

        [Required]
        public int Aantal { get; set; }     
    }
}