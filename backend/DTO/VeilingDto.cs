using System.ComponentModel.DataAnnotations;

namespace backend.DTO
{
    public class VeilingDto
    {
        [Required]
        public decimal StartPrijs { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int TimerInSeconden { get; set; }

        [Required]
        public DateTime StartTimestamp { get; set; }

        [Required]
        public int Hoeveelheid { get; set; }

        [Required]
        public decimal MinPrijs { get; set; }
    }
}