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
        [Range(0.01, double.MaxValue)]
        public decimal PrijsStap { get; set; }

        [Required]
        public int TimerInSeconden { get; set; }
    }
}