using System.ComponentModel.DataAnnotations;

namespace backend.Models.DTO
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