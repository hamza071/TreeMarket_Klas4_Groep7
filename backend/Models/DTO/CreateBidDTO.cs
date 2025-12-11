using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTO
{
    public class CreateBidDTO
    {
        [Required]
        public int VeilingID { get; set; }

        [Required]
        public decimal Bod { get; set; }
    }
}