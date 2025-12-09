using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class CreateBidDTO
    {
        [Required]
        public int VeilingID { get; set; }

        [Required]
        public decimal Bod { get; set; }
    }
}