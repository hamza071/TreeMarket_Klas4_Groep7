using System.ComponentModel.DataAnnotations;

namespace TreeMarket_Klas4_Groep7.Models.DTO
{
    public class VeilingDto
    {
        [Required]
        public decimal StartPrijs { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int TimerInSeconden { get; set; }
    }
}