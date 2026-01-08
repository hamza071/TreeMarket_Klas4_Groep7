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
        public int TimerInSeconden { get; set; } // duur van de veiling

        [Required]
        public DateTime StartTimestamp { get; set; } // geplande starttijd of nu

        [Required]
        public int Hoeveelheid { get; set; }

        [Required]
        public decimal MinPrijs { get; set; }
    }
}