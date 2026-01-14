using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class CreateBidDTO
    {
        [Required]
        public int VeilingID { get; set; }

        [Required]
        public decimal Bod { get; set; }
    }
}